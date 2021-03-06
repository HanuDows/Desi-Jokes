﻿using HanuDowsFramework;
using System;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;

namespace DesiJokes_BackgroundTasks
{
    public sealed class NotificationBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            RawNotification notification = (RawNotification)taskInstance.TriggerDetails;

            _deferral = taskInstance.GetDeferral();

            XDocument xdoc = XDocument.Parse(notification.Content);
            string task = xdoc.Root.Element("Task").Value;

            HanuDowsApplication app = HanuDowsApplication.getInstance();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (task.Equals("PerformSync"))
            {
                int count = 0;
                XElement xe = xdoc.Root.Element("LatestDataTimeStamp");

                if (xe == null)
                {
                    // Old Code --- Perform Synchronization
                    count = await app.PerformSync();
                }
                else
                {
                    string latest_data_timestamp_string = xe.Value;
                    DateTime latest_data_timestamp = DateTime.Parse(latest_data_timestamp_string);

                    DateTime lastSyncTime = DateTime.Parse(localSettings.Values["LastSyncTime"].ToString());

                    int diff = latest_data_timestamp.CompareTo(lastSyncTime);
                    if (diff >= 0)
                    {
                        // Perform Sync
                        count = await app.PerformSync();
                    }
                }

                if (count > 0)
                {
                    string title = "New Jokes downloaded";
                    string message = count + " new jokes have been downloaded.";
                    showToastNotification(title, message);
                }
            }

            if (task.Equals("ShowInfoMessage"))
            {

                string mid = xdoc.Root.Element("MessageID").Value;
                string title = xdoc.Root.Element("Title").Value;
                string content = xdoc.Root.Element("Content").Value;

                var last_mid = localSettings.Values["ToastMessageID"];

                if (last_mid != null && last_mid.Equals(mid))
                {
                    return;
                }

                localSettings.Values["ToastMessageID"] = mid;

                showInfoMessage(title, content);
            }

            if (task.Equals("DeletePostID"))
            {
                int id = (int)xdoc.Root.Element("PostID");
                app.DeletePostFromDB(id);
            }

            if (task.Equals("SyncAllAgain"))
            {
                // Set last sync time to Hanu Epoch
                localSettings.Values["LastSyncTime"] = (new DateTime(2011, 11, 4)).ToString();
                await app.PerformSync();
            }

            _deferral.Complete();
        }

        private void showToastNotification(string title, string content)
        {
            // Get Template
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;

            // Put text in the template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(content));

            // Set the Duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            // Create Toast and show
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

        }

        private void showInfoMessage(string title, string content)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["ToastMessageTitle"] = title;
            localSettings.Values["ToastMessageContent"] = content;

            // Get Template
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;

            // Put text in the template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(content));

            // Set the Duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "long");

            // Show custom Text
            var toastNavigationUriString = "ShowInfoMessage";
            XmlElement toastElement = ((XmlElement)toastXml.SelectSingleNode("/toast"));
            toastElement.SetAttribute("launch", toastNavigationUriString);

            // Create Toast and show
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

        }
    }
}
