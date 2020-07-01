There are different options available which you can leverage to build the API backend. The recommended one from Amazon is AWS Lambda. Since my purpose for this blog is to exploit ways how we can implement Alexa skill, I am using Azure Functions to build the API backend. If you are not aware of <a href="https://azure.microsoft.com/en-us/services/functions/">Azure Function</a>its is a serverless offering from Microsoft Azure. The codebase I am portraying below is available in the Github link is in the end.

<img class="alignnone size-full wp-image-4555" src="https://khanasif1.files.wordpress.com/2020/06/11-e1593582381834.png" alt="11" width="1014" height="443" />

Just to bring in the context today we are talking about the middle segment which our API layer includes Azure Function, Application Insight, and Azure SQL DB.

I have build 3 main endpoints:
<ul>
	<li><strong>nebulaswitchping:  </strong>This endpoint is just used for verifying the availability of the API platform. It's vital for development and BAU support as one can validate if Azure Function is up and running using this

endpoint.</li>
	<li><strong>nebulaswitch: </strong>This endpoint is the core that will serve the request coming from Alexa device to skill and then from skill to API endpoint. This endpoint also updates the switch status in the database based on users' intend.
<ul>
	<li>This method has core dependency on <a href="https://github.com/timheuer/alexa-skills-dotnet">Alexa.NET</a>, this package had a core library which can handle Alexa skill request/response.</li>
	<li>This snippet is invoked when we enable Alexa skill from the device, by speaking "Alexa Open Nebula Switch"
</li>
	<li>Next, all the request go to Intent snippet as below for processing
</li>
	<li>Last I have also included Azure Application Insight for managing application-level telemetry. Below code, snippet calls the helper class AppInsightHelper.cs and pushed the telemetry to Azure Application Insight. Before you run the application please provide <strong><span style="color: #ff0000;">Application Insight Key</span></strong> to the marked section in AppInsightHelper.cs
</li>
</ul>
</li>
	<li><strong>getnebulaswitchstatus: </strong>This endpoint is used by the IoT device to get the updated status of the switch from DB. Details around IoT will be available in <del>Part 3 of the blog series</del></li>
</ul>
