﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.ApplicationServices;
using RudeFox.Helpers;
using RudeFox.Views;
using RudeFox.Services;
using NLog.Targets;
using RudeFox.Updater;
using RudeFox.Enums;

namespace RudeFox.ApplicationManagement
{
    public class SingletonManager : WindowsFormsApplicationBase
    {
        public SingletonManager()
        {
            IsSingleInstance = true;
        }

        protected override bool OnStartup(StartupEventArgs e)
        {
            // first time app is launched
            App.Instance.RegisterExceptionHandlingEvents();
            InitializeComponents();
            App.Instance.Run();
            return false;
        }

        protected override async void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            // subsequent launches
            App.Instance.Activate();

            if (eventArgs.CommandLine.Count() > 0)
                await App.Instance.DeleteFilesOrFolders(eventArgs.CommandLine);
        }

        private void InitializeComponents()
        {
#if !DEBUG
            if (string.IsNullOrWhiteSpace(Keys.DROPBOX_API_KEY))
                DialogService.Instance.GetMessageDialog("API key not found", "Could not find Dropbox API key.", "Ok").ShowDialog();
            else
            {
                UpdateManager.Initialize(Keys.DROPBOX_API_KEY);
                Task.Run(() => App.Instance.UpdateAfter(5));
            }

            // register sentry as NLog target
            Target.Register<Nlog.SentryTarget>("Sentry");
#endif
        }
    }
}
