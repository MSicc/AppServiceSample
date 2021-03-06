﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace SampleAppService
{
    public sealed class Handler :IBackgroundTask
    {

        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private AppServiceConnection _appServiceConnection;


       public void Run(IBackgroundTaskInstance taskInstance)
        {
            //get the task instance deferral
            this._backgroundTaskDeferral = taskInstance.GetDeferral();

            //hooking up to the Canceled event to close app connection
            taskInstance.Canceled += OnTaskCanceled;

            //getting the AppServiceTriggerDetails and hooking up to the RequestReceived event
            var details = (AppServiceTriggerDetails)taskInstance.TriggerDetails;
            _appServiceConnection = details.AppServiceConnection;
            _appServiceConnection.RequestReceived += OnRequestReceived;
        }



        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //async operation needs a deferral
            var msgDeferral = args.GetDeferral();


            //picking up the data ValueSet
            var input = args.Request.Message;
            var result = new ValueSet();

            //parsing the ValueSet
            var response = (string) input["question"];

            try
            {
                //as long as the app service connection is established we are using the same instance even if data changes.
                //to avoid crashes, clear the result before getting the new one
                result.Clear();

                if (!string.IsNullOrEmpty(response))
                {
                    var responderResponse = AppServiceResponder.Responder.Instance.GetResponse(response);

                    result.Add("Status", "OK");
                    result.Add("response", responderResponse);
                }

                await args.Request.SendResponseAsync(result);
            }
            //using finally because we need to tell the OS we have finished, no matter of the result
            finally
            {
                msgDeferral?.Complete();
            }


        }


        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //we need to tell the OS we have finished also if our background task gets cancelled for whatever reason
            this._backgroundTaskDeferral?.Complete();


            //todo: implement a logging mechanism for the BackgroundTaskCancellationReason 
        }
    }
}
