using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using XIDNA.Models;

namespace XIDNA.OneSignalR
{
    public class OneHub:Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.sendMessage(name, message);
        }
        public void Update(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void LeaveGroup(string groupName)
        {
            Groups.Remove(Context.ConnectionId, groupName);
        }

        public void SendMessageToGroup(string groupName, string name, string message)
        {
            try
            {
                Clients.Group(groupName).sendMessage(name, message);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateChatInfo(string message, string sConnectionID)
        {
            try
            {
                Clients.Client(sConnectionID).UpdateChatInfo(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool bISDisconnect)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            return base.OnDisconnected(bISDisconnect);
        }
        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }
    }
}