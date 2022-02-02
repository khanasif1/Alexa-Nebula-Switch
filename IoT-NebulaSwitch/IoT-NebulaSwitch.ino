#include <Dhcp.h>
#include <Dns.h>
#include <Ethernet.h>
#include <EthernetClient.h>
#include <EthernetServer.h>
#include <EthernetUdp.h>
#include "oled.h"
/*
    This sketch sends data via HTTP GET requests to data.sparkfun.com service.

    You need to get streamId and privateKey at data.sparkfun.com and paste them
    below. Or just customize this script to talk to other HTTP servers.

  #define D0 16
  #define D1 5 // I2C Bus SCL (clock)
  #define D2 4 // I2C Bus SDA (data)
  #define D3 0
  #define D4 2 // Same as "LED_BUILTIN", but inverted logic
  #define D5 14 // SPI Bus SCK (clock)
  #define D6 12 // SPI Bus MISO
  #define D7 13 // SPI Bus MOSI
  #define D8 15 // SPI Bus SS (CS)
  #define D9 3 // RX0 (Serial console)
  #define D10 1 // TX0 (Serial console)
*/
//#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <WiFiManager.h>
#include <WiFiClientSecure.h>
#include <Streaming.h>
#include <ArduinoJson.h>
#include "WifiHelper.h"

long apiCallInterval = 0;
int ApiCallInterval = 30000; //Call API every minute
char* host = "nebulaswitch.azurewebsites.net";
const int httpPort = 80;
String url = "/api/NebulaStatus/GetSwtichStatus";
int RelayPin = 12;
bool RelayState = false;
int switchState = 0;
bool firstRun = true;

int restartInterval = 60 * 60 * 1000; //Restart every hour
long restartTicker = 0;

int wifiFailCnt = 0;
int wifiMaxRetry = 5;
long apiRetry = 0;

void setup() {
  oled.DisplayMessage("System Initialized", 2, 0, 3);
  Serial.println("***********************Setup Begins*********************** ");
  Serial.begin(9600);
  Serial.println("***********************Connected Started*********************** ");
  delay(5000);
  //oled.DisplayMessage("Wifi Strat", 2, 0, 3);
  WifiHelper.Connect();
  //oled.DisplayMessage("Wifi Connected", 2, 0, 3);
  delay(500);
  Serial.println("***********************Connected to WiFi*********************** ");
  pinMode(2, OUTPUT);
  pinMode(RelayPin, OUTPUT); //Relay Switch Pin
}

void loop() {
  if (firstRun == true) {
    Serial.println("In first run");
    firstRun = false;
    oled.DisplayMessage("SWITCH OFF", 2, 0, 3);
  }
  if (millis() - apiCallInterval > ApiCallInterval) {
    //oled.DisplayMessage("Start API Call", 2, 0, 3);
    /*Call API every 20sec, millis give time im millisec since board started*/
    digitalWrite(2, LOW);   // Turn the LED on by making the voltage LOW
    delay(1000);            // Wait for a second
    Serial.println("API Calling to get status");
    String rawswitchState = HTTPRequestHelper();

    rawswitchState.trim();
    Serial.println("API Response : " + rawswitchState);
    Serial.println("API Response length : " + rawswitchState.length());

    if (rawswitchState.length() == 0) {
      apiRetry++;
      while (apiRetry < 2) {
        delay(30000);
        Serial.println("In API Retry : " + apiRetry);
        String rawswitchState = HTTPRequestHelper();
        rawswitchState.trim();
        Serial.println("API Retry result length: ");
        Serial.println(rawswitchState.length());
        if (rawswitchState.length() != 0) {
          break;
        } else {
          Serial.println("API Retry count :" + apiRetry);
          Serial.println(apiRetry);
        }
      }
    }

    if (rawswitchState.length() != 0) {
      switchState = rawswitchState.toInt();
      if (switchState == 1) {
        Serial.println("Switch ON");
        if (RelayState == false) {
          oled.DisplayMessage("SWITCH ON", 2, 0, 3);
          digitalWrite(RelayPin, LOW);       // sets the digital pin 13 ON
          RelayState = true;
        } else {
          Serial.println("Already on");
        }
      } else if (switchState == 0) {
        Serial.println("Switch OFF");
        if (RelayState == true) {
          oled.DisplayMessage("SWITCH OFF", 2, 0, 3);
          digitalWrite(RelayPin, HIGH);       // sets the digital pin 13 OFF
          RelayState = false;
        } else {
          Serial.println("Already off");
        }
      } else {
        Serial.println("Some error at API side, should be good in next run");
        RelayState == false;
      }
    } else {
      Serial.println("Some error at API side, should be good in next run");
      digitalWrite(RelayPin, HIGH);//***********Remove***********
      digitalWrite(RelayPin, LOW); //***********Remove***********
    }
    Serial.println("API Call end");
    digitalWrite(2, HIGH);  // Turn the LED off by making the voltage HIGH

    apiCallInterval = millis();
  }
}


String HTTPRequestHelper() {
  Serial.println("in HTTP Helper");
  // Use WiFiClient class to create TCP connections
  WiFiClient client;
  const int httpPort = 80;

  if (!client.connect(host, httpPort)) {
    wifiFailCnt++;
    Serial.println("connection to server failed");
    if (wifiFailCnt > wifiMaxRetry) {
      Serial.println("WiFi connection attempt exceeded max count. Starting Reconnect!!!");
      wifiFailCnt = 0;
      WifiHelper.Connect();
    } else {
      Serial.println("WiFi reconnect time not met!!");
      return String(switchState);
    }
  } else {
    Serial.println("connection to server success :)`");
  }
  Serial.println("Is Connected");

  String data = "2";
  String jsonMessage = "";
  client.print(String("GET ") + url + " HTTP/1.1\r\n" +
               "Host: " + host + "\r\n" +
               "Content-Type: application/json\r\n" +
               "Content-Length: " + data.length() + "\r\n" +
               "\r\n" + // This is the extra CR+LF pair to signify the start of a body
               data + "\n");
  Serial.println("Get Request executed");
  unsigned long timeout = millis();
  while (client.available() == 0) {
    if (millis() - timeout > 90000) {
      Serial.println(">>> Client Timeout !");
      client.stop();
      break;
    }
  }
  Serial.println("Reading HTTP Result");
  // Read all the lines of the reply from server and print them to Serial
  while (client.available()) {
    jsonMessage = client.readStringUntil('\r');
  }

  Serial.println("Response : " + jsonMessage);
  wifiFailCnt = 0; //Alway set wifi fail count to 0 if API call was success
  return jsonMessage;
}
