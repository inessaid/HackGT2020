/*
 * Copyright 2019 Scott Hwang. All Rights Reserved.
 * This code was modified from ExampleAssistantV2.cs 
 * in unity-sdk-4.0.0. This continues to be licensed 
 * under the Apache License, Version 2.0 as noted below.
 */

/**
* Copyright 2018 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/
#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class SimpleBot : MonoBehaviour
{
    [Button(ButtonSizes.Large)]
    public void SayHello()
    {
        Debug.Log("Hello button!");
    }

  //  public spawner spawnerDict;

    public GameObject[] spawnerDict;
    public Dictionary<string, GameObject[]> spawnDict = null;

    private Dictionary<string, GameObject[]> spawnlist;

    [SerializeField]
    private WatsonSettings settings;

    private AssistantService Assistant_service;

    private bool createSessionTested = false;

    // 2020/2/15 Changed to protected to be accessible in CommandBot.cs
    protected bool messageTested = false;
    private bool deleteSessionTested = false;
    private string sessionId;

    public string textResponse = String.Empty;

    [SerializeField]
    private InputField targetInputField;

    [SerializeField]
    private Text targetText;

    public GameObject[] spawnItems;

    public Transform Playerlocation;

    //Keep track of whether IBM Watson Assistant should process input or is
    //processing input to create a chat response.
    public enum ProcessingStatus { Process, Processing, Idle, Processed };
    private ProcessingStatus chatStatus;

    public enum InputFieldTrigger { onValueChanged, onEndEdit };

    [SerializeField]
    private InputField externalInputField;
    [SerializeField]
    private InputFieldTrigger externalTriggerType;

    private InputField inputField;

    // The output target GameObject to receive text from this gameObject.
    [SerializeField]
    private GameObject targetGameObject;

    private void Start()
    {
      

       

        // Enable TLS 1.2
        //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

        // Disable old protocols
        //ServicePointManager.SecurityProtocol &= ~(SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11);

        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());
        chatStatus = ProcessingStatus.Idle;

        // Since coroutines can't return values I use the onValueChanged listener
        // to trigger an action after waiting for an input to arrive.
        // I originally used enums or flags to keep track if a process such
        // as obtaining a chat response from IBM Assistant was still being processed
        // or was finished processing but this was cumbersome.

        if (externalInputField != null)
        {
            if (externalTriggerType == InputFieldTrigger.onEndEdit)
            {
                externalInputField.onEndEdit.AddListener(delegate { Runnable.Run(ProcessChat(externalInputField.text)); });
            }
            else
            {
                externalInputField.onValueChanged.AddListener(delegate { Runnable.Run(ProcessChat(externalInputField.text)); });
            }
        }

        inputField = gameObject.AddComponent<InputField>();
        inputField.textComponent = gameObject.AddComponent<Text>();
        inputField.onValueChanged.AddListener(delegate { Runnable.Run(ProcessChat(inputField.text)); });
    }

    public IEnumerator CreateService()
    {

        if (string.IsNullOrEmpty(settings.Assistant_apikey))
        {
            throw new IBMException("Please provide Watson Assistant IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        //            IamAuthenticator authenticator = new IamAuthenticator(apikey: Assistant_apikey, url: serviceUrl);
        IamAuthenticator authenticator = new IamAuthenticator(apikey: settings.Assistant_apikey);

        //  Wait for tokendata
        while (!authenticator.CanAuthenticate())
            yield return null;

        Assistant_service = new AssistantService(settings.versionDate, authenticator);
        if (!string.IsNullOrEmpty(settings.serviceUrl))
        {
            Assistant_service.SetServiceUrl(settings.serviceUrl);
        }

        Assistant_service.CreateSession(OnCreateSession, settings.assistantId);

        while (!createSessionTested)
        {
            yield return null;
        }
    }

    // Get the "welcome" chat reponse from IBM Watson Assistant
    public IEnumerator Welcome()
    {
        // Set chat processing status to "Processing"
        chatStatus = ProcessingStatus.Processing;

        while (!createSessionTested)
        {
            yield return null;
        }

        Assistant_service.Message(OnMessage, settings.assistantId, sessionId);
        while (!messageTested)
        {
            messageTested = false;
            yield return null;
        }

        if (!String.IsNullOrEmpty(textResponse))
        {
            // Set status to show chat processing has finished 
            chatStatus = ProcessingStatus.Processed;
        }

    }

    public void GetChatResponse(string chatInput)
    {
        StartCoroutine(ProcessChat(chatInput));
    }

    public IEnumerator ProcessChat(string chatInput)
    {
        Debug.Log("Processchat: " + chatInput);

        // Set status to show that the chat input is being processed.
        chatStatus = ProcessingStatus.Processing;

        if (Assistant_service == null)
        {
            yield return null;
        }
        if (String.IsNullOrEmpty(chatInput))
        {
            yield return null;
        }

        messageTested = false;
        var inputMessage = new MessageInput()
        {
            Text = chatInput,
            Options = new MessageInputOptions()
            {
                ReturnContext = true
            }
        };

        Assistant_service.Message(OnMessage, settings.assistantId, sessionId, input: inputMessage);

        while (!messageTested)
        {
            messageTested = false;
            yield return null;
        }

        if (!String.IsNullOrEmpty(textResponse))
        {
            // Set status to show chat processing has finished.
            chatStatus = ProcessingStatus.Processed;
        }
    }

    private void OnDeleteSession(DetailedResponse<object> response, IBMError error)
    {
        deleteSessionTested = true;
    }

    // 2020/2/15 - changed to protected virtual to allow for inheritance.
    // This is where the returned chat response is set to send as output
    protected virtual void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
    {
        textResponse = response.Result.Output.Generic[0].Text.ToString();
        Debug.Log(textResponse);
        messageTested = true;
        if (targetInputField != null)
        {
            targetInputField.text = textResponse;
           
        }

        // Check if the target GameObject has an InputField then place text into it.
        if (targetGameObject != null)
        {
            InputField target = targetGameObject.GetComponent<InputField>();
            if (target != null)
            {
                target.text = textResponse;
            }
        }

        if (response != null && response.Result.Output.Intents.Count != 0)
        {
            string intent = response.Result.Output.Intents[0].Intent;
            Debug.LogError("HERE IS THE INTENT : " + intent);
          
            bool createdObject = false;

            GameObject myObject = null;
            Renderer rend = null;
            string ObjectType = null;
            Color ObjectColor = Color.red;
            string currentMat = null;

            if(intent == "create")
            {
                foreach (RuntimeEntity entity in response.Result.Output.Entities)
                {
                    Debug.LogError("entityType: " + entity.Entity + " , value: " + entity.Value);
                    // direction = entity.Value;
                    //gameManager.MoveObject(direction);

                    if (entity.Entity == "object")
                    {
                   
                        //gameManager.CreateObject(entity.Value, currentMat, currentScale);
                        createdObject = true;
                        currentMat = null;
                        //currentScale = null;
                        ObjectType = entity.Value;

                    }


                    if (entity.Entity == "material")
                    {
                        currentMat = entity.Value;

                        if (currentMat == "black")
                        {
                            ObjectColor = Color.black;
                        }
                        else if (currentMat == "blue")
                        {
                            ObjectColor = Color.blue;
                        }
                        else if (currentMat == "cyan")
                        {
                            ObjectColor = Color.cyan;
                        }
                        else if (currentMat == "gray")
                        {
                            ObjectColor = Color.gray;
                        }
                        else if (currentMat == "green")
                        {
                            ObjectColor = Color.green;
                        }
                        else if (currentMat == "magenta")
                        {
                            ObjectColor = Color.magenta;
                        }
                        else if (currentMat == "red")
                        {
                            ObjectColor = Color.red;
                        }
                        else if (currentMat == "white")
                        {
                            ObjectColor = Color.white;
                        }
                        else if (currentMat == "yellow")
                        {
                            ObjectColor = Color.yellow;
                        }


                    }


                    if (ObjectType != null)
                    {
                        var playerPosition = FindObjectOfType<OVRPlayerController>().GetComponent<Transform>();
                        Vector3 objectPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, playerPosition.position.z + 2f);


                        if (ObjectType == "cube")
                        {
                            myObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            myObject.transform.position = new Vector3(0f, 1f, 0f);
                            rend = myObject.GetComponent<Renderer>();
                            rend.material.color = Color.red;
                        }
                        else if (ObjectType == "ball")
                        {

                            Instantiate(spawnItems[0], objectPosition, Quaternion.identity);
                        }
                        else if (ObjectType == "bottle")
                        {

                             Instantiate(spawnItems[1], objectPosition, Quaternion.identity);
                        }
                        else if (ObjectType == "bed")
                        {

                             Instantiate(spawnItems[2], objectPosition, Quaternion.identity);
                        }

                        rend = myObject.GetComponent<Renderer>();
                        rend.material.color = ObjectColor;
                    }

                }
                if (!createdObject)
                {
                    //gameManager.PlayError(sorryClip);
                }

            }
            else if (intent == "destroy")
            {
                //gameManager.DestroyAtPointer();
            }
            else if (intent == "help")
            {

            }
        }
        else
        {
            Debug.Log("Failed to invoke OnMessage();");
        }
    
           

        }

        
    

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        Log.Debug("SimpleBot.OnCreateSession()", "Session: {0}", response.Result.SessionId);
        sessionId = response.Result.SessionId;
        createSessionTested = true;
    }

    public void SetChatStatus(ProcessingStatus status)
    {
        chatStatus = status;
    }

    public ProcessingStatus GetStatus()
    {
        return chatStatus;
    }

    public bool ServiceReady()
    {
        return createSessionTested;
    }

    public string GetResult()
    {
        chatStatus = ProcessingStatus.Idle;
        return textResponse;
    }

}

