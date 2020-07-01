To configure Alexa Skill as I discussed in the User Interface section, you can follow easy steps below:
<ol>
	<li>Sign in to the Alexa development portal <a href="https://developer.amazon.com/">https://developer.amazon.com/</a>. You can create a free Amazon account if you don't have one.
<img class="alignnone size-full wp-image-4544" src="https://khanasif1.files.wordpress.com/2020/06/1.png" alt="1" width="418" height="558" /></li>
	<li>Create a new skill using Create Skill option in Alexa Development Console. Also choose Custom template option from tiles as below.
<img class="alignnone size-full wp-image-4545" src="https://khanasif1.files.wordpress.com/2020/06/2.png" alt="2" width="936" height="934" /></li>
	<li>Once your new skill project is created you will be navigate to screen as below. One of the most important item to consider in previous step is selecting right Default language, which should be based on your physical location. If you don't select language as per your location you will not be able to invoke the skill from Alexa device when in development stage.
<img class="alignnone size-full wp-image-4546" src="https://khanasif1.files.wordpress.com/2020/06/3.png" alt="3" width="1672" height="797" /></li>
	<li>We need to provide an <strong>Invocation</strong> name. This is crucial as this will help Alexa skill back end identify your skill from a large skills data repository. In the real world when you will talk to an Alexa device like an echo dot or echo speaker or any of the Alexa devices, and say "Alexa !! open {your skill name}" Alexa skills service will look for invocation name to process request.
<img class="alignnone size-full wp-image-4547" src="https://khanasif1.files.wordpress.com/2020/06/4.png" alt="4" width="1150" height="375" /></li>
	<li>Next, we need to set up an Intent. <strong>Intent</strong> is the user interaction model that the skill will be aligned to. There is a long list of predefined Intent available, and you have full privilege of custom building one of your own.
<img class="alignnone size-full wp-image-4548" src="https://khanasif1.files.wordpress.com/2020/06/5.5.png" alt="5.5" width="719" height="331" /></li>
	<li>Below is a list of some default Intents available, for this blog I will be creating a custom Intent which I will then extend to carry input parameters for processing by Alexa Skill backend API.
<img class="alignnone size-full wp-image-4549" src="https://khanasif1.files.wordpress.com/2020/06/5.png" alt="5" width="1036" height="574" /></li>
	<li>Within the Intent, we need to define Utterances. This is a set of likely spoken phrases mapped to the intents. This should include as many representative phrases as possible
<img class="alignnone size-full wp-image-4550" src="https://khanasif1.files.wordpress.com/2020/06/6.png" alt="6" width="1826" height="776" /></li>
	<li>In the Utterances we can add <strong>Slots, </strong>slots can be understood as a dictionary of different types such as Airlines, Cities, Countries, States, Artists, Colour, etc. Alexa has a long list of such types available, also if you don't find one suitable. You can use <strong>AMAZON.SearchQuery </strong>which I am using in the blog. This slot type will pass the user spoken word as is to skill API backend.
<img class="alignnone size-full wp-image-4551" src="https://khanasif1.files.wordpress.com/2020/06/7.png" alt="7" width="993" height="604" /></li>
	<li>If you complete till step 8 you are done with basic configuration. You can validate the configuration as JSON using JSON editor available.
<img class="alignnone size-full wp-image-4552" src="https://khanasif1.files.wordpress.com/2020/06/8.png" alt="8" width="1175" height="800" /></li>
	<li>Now we are done with the skill setup, so next, we need to set up an API endpoint which will process the user income Intents. There are 2 options available at this stage to build Backend Service Endpoints
<ul>
	<li><strong>AWS Lambda ARN</strong></li>
	<li><strong>HTTPS</strong><strong>
</strong>For this demo, I have used Azure Functions to build my API backend to server requests. In Part 2 of the blog, I will explain about the API Endpoints I build using Azure Functions
<img class="alignnone size-full wp-image-4564" src="https://khanasif1.files.wordpress.com/2020/06/9-1.png" alt="9" width="1761" height="849" /></li>
</ul>
</li>
	<li>The last item is to save the model and Build Model. Building the model will generate a Machine Learning Model which will be used by the platform to intercept user utterances and extract slot values to process.
<img class="alignnone size-full wp-image-4554" src="https://khanasif1.files.wordpress.com/2020/06/10.png" alt="10" width="1134" height="289" /></li>
</ol>
With these 11 steps, we are done with the setup and configuration for Alexa Skill.  Next, we need to build the API endpoints to process user requests. The next part of this blog series will cover Azure Functions based API which is used in step 10.
