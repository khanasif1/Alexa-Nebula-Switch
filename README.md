# Alexa Nebula Switch
Nebula switch is a Alexa operated IoT switch to manage electrical applicances.
<img class="alignnone size-full wp-image-4556" src="https://khanasif1.files.wordpress.com/2020/06/maxresdefault.jpg" alt="maxresdefault" width="1280" height="720" />

It's no more a matter of discussion that voice-enabled systems are future. In or to keep up in the game it's always good to understand the technology and try your hands on it.

In order to get my hands dirty, I tried to build a solution using Alexa and a couple of other components. The endstate is the user can speak to Alexa device to control one or more devices by turning them on or off. I know there are solutions available in the market off the shelf, the reason for reinventing this is those solutions are way too expensive then than I can quantify. So I build this $25 solution which does the same and many more actions as those market solutions and has great capability to extend.
<img class="alignnone size-full wp-image-4555" src="https://khanasif1.files.wordpress.com/2020/06/11.png" alt="11" width="1014" height="443" />

The solution comprises of three main components:
<ul>
	<li><strong>User Interface</strong>: This component includes an Alexa Echo Dot and Alexa Skill</li>
	<li><strong>Server</strong>: We are using a couple of services to build the API backend which Alexa skill can contact to process requests. We will discuss this segment in the next part</li>
	<li><strong>Home IoT Devices:</strong> We are using ESP8266 as an IoT device to get updates from API backend and update switch which changes the device state attached to switch i.e. turning on or off.</li>
</ul>
