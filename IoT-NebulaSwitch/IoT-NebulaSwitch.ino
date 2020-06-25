#include <Dhcp.h>
#include <Dns.h>
#include <Ethernet.h>
#include <EthernetClient.h>
#include <EthernetServer.h>
#include <EthernetUdp.h>

/*
 *  This sketch sends data via HTTP GET requests to data.sparkfun.com service.
 *
 *  You need to get streamId and privateKey at data.sparkfun.com and paste them
 *  below. Or just customize this script to talk to other HTTP servers.
 * 
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
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <WiFiManager.h>  
#include <WiFiClientSecure.h>
#include <Streaming.h> 
#include <ArduinoJson.h>
#include "WifiHelper.h"

long apiCallInterval = 0;
char* host = "nebula-switch.azurewebsites.net";
const int httpPort = 80;
String url = "/api/getnebulaswitchstatus";


void setup() {
  Serial.println("***********************Setup Begins*********************** ");
  Serial.begin(9600);
  delay(100);  
  Serial.println("***********************Connected Started*********************** ");
  WifiHelper.Connect();        
  delay(500); 
  Serial.println("***********************Connected to WiFi*********************** ");  
  pinMode(2, OUTPUT);
  pinMode(13, OUTPUT); //Relay Switch Pin
}

void loop() {
  if(millis()- apiCallInterval>20000){
    /*Call API every 20sec, millis give time im millisec since board started*/
    digitalWrite(2, LOW);   // Turn the LED on by making the voltage LOW
    delay(1000);            // Wait for a second
    Serial.println("API Calling to get status");
    String switchState= HTTPRequestHelper();
    if(switchState.toInt()==1){
      Serial.println("Switch ON");
      digitalWrite(13, LOW);       // sets the digital pin 13 ON
    }else{
      Serial.println("Switch OFF");
      digitalWrite(13, HIGH);       // sets the digital pin 13 OFF 
    }
    
    Serial.println("API Call end");    
    digitalWrite(2, HIGH);  // Turn the LED off by making the voltage HIGH
    delay(2000);
    apiCallInterval=millis();
  }
}

String HTTPRequestHelper(){
  Serial.println("in HTTP Helper");
    // Use WiFiClient class to create TCP connections
  WiFiClient client;
  const int httpPort = 80;
  
  if (!client.connect(host, httpPort)) {
    //DisplayMessage("connection to server failed");
    Serial.println("connection to server failed");
     WifiHelper.Connect();  
    //return "";
  }
  Serial.println("Is Connected");
  
  String data="2";
  String jsonMessage="";
   client.print(String("GET ") + url + " HTTP/1.1\r\n" +
                 "Host: " + host + "\r\n" +                
                 "Content-Type: application/json\r\n" +
                 "Content-Length: " + data.length() + "\r\n" +
                 "\r\n" + // This is the extra CR+LF pair to signify the start of a body
                 data +"\n");
     Serial.println("Get Request executed");
      unsigned long timeout = millis();
      while (client.available() == 0) {
        if (millis() - timeout > 10000) {
          Serial.println(">>> Client Timeout !");          
          client.stop();        
        }
      }
      Serial.println("Reading HTTP Result");   
      // Read all the lines of the reply from server and print them to Serial
      while(client.available()){
        jsonMessage = client.readStringUntil('\r');             
      }   
      Serial.println("Response : "+jsonMessage);
      return jsonMessage;
}
