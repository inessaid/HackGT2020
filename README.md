# Create Your Own VR Enviornment Using Voice Commands and Controllers Input with IBM Watson Speech-to-Text, Assistant, and Text-To-Speech on Unity

Austin Stanbury, Ines Said, Patrick Molen, Rashaan Siddiqui

10/18/2020

# Introduction

This projects implements three IBM Watson Services: Speech-To-Text, Text-To-Speech and Assistant!

# Setting up IBM Watson Cloud Services

Start by creating your IBM Watson Speech-To-Text, Text-To-Speech and Assistant.

Follow the steps below to find your creditionals:

##### Speech to Text and Text to Speech

1- Go to your resource list by clicking the panel at the top left of your screen.

2- Under services, click the service you want to get creditionals for.

3- Navigate to Manage and under Credentials, you can find API key and URL (See Below).

<img src="https://media.discordapp.net/attachments/766491669590835231/767310797675167754/Capture.PNG?width=1208&height=460"
     alt="Markdown Monster icon"
     style="float: left; margin-right: 10px;" />
     
#### IBM Assistant

1- Go to your resource list by clicking the panel at the top left of your screen.

2- Under services, click IBM Watson Assistant.

3- Navigate to Manage and under Credentials, you can find API key and URL. You only need to get the URL from this page, as it is different from the assistant URL.

4- Click the big blue "Launch Watson Assistant" button.

5- Click the three dots at the side of your assistant (as shown below).


<img src="https://media.discordapp.net/attachments/766491669590835231/767315548920741908/Capture1.PNG?width=1208&height=365"
     alt="Markdown Monster icon"
     style="float: left; margin-right: 10px;" />
     
6- Click on Settings.

7- Navigate to API details on the left of the window. In this page, you can see all the credentials needed.

WARNING !! THE COPY BUTTON DOES NOT WORK ON THE ASSISTANT PAGE. MAKE SURE TO HIGHLIGHT THE CREDENTIALS AND MANUALLY COPY AND PASTE THEM.

<img src="https://cdn.discordapp.com/attachments/766491669590835231/767326008553766973/Capture.PNG"
     alt="Markdown Monster icon"
     style="float: left; margin-right: 10px;" />

Once you have your credentials saved, make sure to add them to Watson Settings in Unity:

1- Search Watson Settings on Unity search bar.

2- Select the red Icon.

3- Paste credentials.

# Setting Up Oculus

Oculus APK is already included in this repo. There is no need to import it again. Make sure VR is still enabled in your project by following these steps:

1- Navigate to Unity Build Settings, Player Settings, XR settings

2- Make sure Oculus is added to the Virtual Reality SDKs, otherwise click the + button and add it.

3- Make sure Shared Depth Buffer and Dash Support are checked.

<img src="https://media.discordapp.net/attachments/766491669590835231/767328712974663690/xr_settings.PNG"
     alt="Markdown Monster icon"
     style="float: left; margin-right: 10px;" />
     
# Problems We're Still Trying to Fix

There are some lag issues if too many huge objects are placed in the scene.

