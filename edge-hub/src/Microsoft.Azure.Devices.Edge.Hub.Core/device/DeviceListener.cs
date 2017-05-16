﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.Azure.Devices.Edge.Hub.Core.Device
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Edge.Hub.Core.Cloud;
    using Microsoft.Azure.Devices.Edge.Util;
    using IMessage = Microsoft.Azure.Devices.Edge.Hub.Core.IMessage;

    class DeviceListener : IDeviceListener
    {        
        readonly IEdgeHub edgeHub;
        readonly IConnectionManager connectionManager;
        readonly ICloudProxy cloudProxy;

        public DeviceListener(IIdentity identity, IEdgeHub edgeHub, IConnectionManager connectionManager, ICloudProxy cloudProxy)
        {
            this.Identity = Preconditions.CheckNotNull(identity);
            this.edgeHub = Preconditions.CheckNotNull(edgeHub);
            this.connectionManager = Preconditions.CheckNotNull(connectionManager);
            this.cloudProxy = Preconditions.CheckNotNull(cloudProxy);            
        }

        public IIdentity Identity { get; }

        public Task<object> CallMethodAsync(string methodName, object parameters, string deviceId)
        {
            throw new NotImplementedException();
        }

        public void BindDeviceProxy(IDeviceProxy deviceProxy)
        {
            ICloudListener cloudListener = new CloudListener(deviceProxy);
            this.cloudProxy.BindCloudListener(cloudListener);
            this.connectionManager.AddDeviceConnection(this.Identity, deviceProxy);
        }

        public Task CloseAsync()
        {
            return this.connectionManager.CloseConnectionAsync(this.Identity.Id);
        }

        public Task ProcessFeedbackMessageAsync(IFeedbackMessage feedbackMessage)
        {
            return this.cloudProxy.SendFeedbackMessageAsync(feedbackMessage);
        }

        public Task<Twin> GetTwinAsync()
        {
            return this.cloudProxy.GetTwinAsync();
        }

        public Task ProcessMessageAsync(IMessage message)
        {
           Preconditions.CheckNotNull(message);
            return this.edgeHub.ProcessDeviceMessage(this.Identity, message);
        }

        public Task ProcessMessageBatchAsync(IEnumerable<IMessage> messages)
        {
            List<IMessage> messagesList = Preconditions.CheckNotNull(messages, nameof(messages)).ToList();
            return this.edgeHub.ProcessDeviceMessageBatch(this.Identity, messagesList);
        }        

        public Task UpdateReportedPropertiesAsync(TwinCollection reportedProperties)
        {
            throw new NotImplementedException();
        }
    }
}