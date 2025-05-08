using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicChat;
using Microsoft.AspNet.SignalR;
using XIDatabase;
using XIDNA.Models;
using XISystem;

namespace BasicChat
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public List<T> GetConnectionKeys()
        {
            return _connections.Keys.ToList();
        }

        public List<HashSet<string>> GetConnectionIds()
        {
            return _connections.Values.ToList();
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }

    public class OrgConnectionMapping<T>
    {
        private readonly Dictionary<T, ConnectionMapping<T>> _connections =
            new Dictionary<T, ConnectionMapping<T>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T orgKey, T roleKey, string connectionId)
        {
            lock (_connections)
            {
                ConnectionMapping<T> roleMappings;
                if (!_connections.TryGetValue(orgKey, out roleMappings))
                {
                    roleMappings = new ConnectionMapping<T>();
                    roleMappings.Add(roleKey, connectionId);
                    _connections.Add(orgKey, roleMappings);
                }

                lock (roleMappings)
                {
                    roleMappings.Add(roleKey, connectionId);
                    _connections[orgKey] = roleMappings;
                }
            }
        }

        public IEnumerable<string> GetConnections(T orgKey, T roleKey)
        {
            ConnectionMapping<T> roleMapping;
            if (_connections.TryGetValue(orgKey, out roleMapping))
            {
                return roleMapping.GetConnections(roleKey);
            }

            return Enumerable.Empty<string>();
        }

        public List<string> GetAllConnectionsInOrg(T orgKey)
        {
            ConnectionMapping<T> roleMapping;
            List<string> connectionIds = new List<string>();
            if (_connections.TryGetValue(orgKey, out roleMapping))
            {
                roleMapping.GetConnectionIds().ForEach(connection => connectionIds.AddRange(connection.ToList()));
            }
            return connectionIds;
        }

        public void Remove(T orgKey, T roleKey,   string connectionId)
        {
            lock (_connections)
            {
                ConnectionMapping<T> roleMappings;
                HashSet<string> connections;
                if (!_connections.TryGetValue(orgKey, out roleMappings))
                {
                    return;
                }

                lock (roleMappings)
                {
                    roleMappings.Remove(roleKey, connectionId);

                    if (roleMappings.Count == 0)
                    {
                        _connections.Remove(orgKey);
                    }
                }
            }
        }
    }
}

namespace XIDataBase.Hubs
{
    [Authorize]
    public class NotifyHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        private readonly static OrgConnectionMapping<string> _orgConnections = new OrgConnectionMapping<string>();
        public void Send(string Message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            // Call the addNewMessageToPage method to update clients.
            context.Clients.All.addNewMessageToPage(Message);
        }

        public void UpdateFeed(string sMessage, string Time, string sMesID, int iLayoutID, string BOInfo, string sOrgs, int Org, string sMode, string sType, bool bUpdate)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            // Call the addNewMessageToPage method to update clients.
            context.Clients.All.UpdateFeed(sMessage, Time, sMesID, iLayoutID, BOInfo, sOrgs, Org, sMode, sType, bUpdate);
        }

        public void Query_Debug(string sQuery, string sIdentity)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            // Call the addNewMessageToPage method to update clients.
            context.Clients.All.QueryDebug(sQuery, sIdentity);
        }
        
        public void NotificationMessages(object Data, string BOName, string HTML)
        {
            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            LeadContext.Clients.All.signalRMessages(Data, BOName, HTML);
        }

        //public void SendNotificationMessages(string sUserName, object Data, string BOName)
        //{
        //    IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();

        //    List<string> userConnections =  _connections.GetConnections(sUserName).ToList();
        //    foreach (string connectionID in userConnections)
        //    {
        //        LeadContext.Clients.Client(connectionID).GetFlashMessages(Data, BOName);
        //    }
        //}

        private List<string> GetRoleNotificationUserNames(List <string> roleConnectionIDs)
        {
            List<string> roleNotifUserNames = new List<string>();
            List<string> connectionKeys = _connections.GetConnectionKeys();

            connectionKeys.ForEach(sConnectionKey =>
            {
                if (_connections.GetConnections(sConnectionKey).ToList().Any(x => roleConnectionIDs.Contains(x)))
                {
                    roleNotifUserNames.Add(sConnectionKey);
                }
            });
            return roleNotifUserNames;
        }

        public List<string> SendNotificationMessages(List<string> userNames, string sRoleIDs, string sOrgID, object Data, string BOName, string sTheme, bool bIsImportant, Guid? FKiLayoutIDXIGUID=null, int? iLeft = null, int? iTop = null, int? iWidth = null, int? iHeight = null)
        {
            List<string> notifUserNames = new List<string>();
            List<string> roleConnectionIDs = new List<string>();

            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            List<string> connectionIDs = new List<string>();

            userNames.ForEach(sUserName =>
            {
                if(_connections.GetConnections(sUserName).ToList().Count > 0)
                {
                    connectionIDs.AddRange(_connections.GetConnections(sUserName).ToList());
                    notifUserNames.Add(sUserName);
                }
            });

            List<string> roleIDS = sRoleIDs?.Split(',').ToList();
            roleIDS?.ForEach(sRoleID => {
                roleConnectionIDs.AddRange(_orgConnections.GetConnections(sOrgID, sRoleID).ToList());
                connectionIDs.AddRange(_orgConnections.GetConnections(sOrgID, sRoleID).ToList());
            });

            connectionIDs = new HashSet<string>(connectionIDs).ToList();

            connectionIDs.ForEach(sConnectionID =>
            {
                LeadContext.Clients.Client(sConnectionID).GetFlashMessages(Data, BOName, sTheme, bIsImportant, FKiLayoutIDXIGUID == Guid.Empty ? null : FKiLayoutIDXIGUID.ToString(), iLeft, iTop, iWidth, iHeight);
            });

            notifUserNames.AddRange(GetRoleNotificationUserNames(roleConnectionIDs));
            notifUserNames = new HashSet<string>(notifUserNames).ToList();

            return notifUserNames;

        }

        public void SendUpdateNotificationCountSignal(List<string> userNames, int iNotificationCategory)
        {
            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            userNames.ForEach(sUserName =>
            {
                if (_connections.GetConnections(sUserName).ToList().Count > 0)
                {
                    var connectionIDs = _connections.GetConnections(sUserName).ToList();
                    connectionIDs = new HashSet<string>(connectionIDs).ToList();
                    connectionIDs.ForEach(sConnectionID =>
                    {
                        LeadContext.Clients.Client(sConnectionID).UpdateHeaderNotificationCount(iNotificationCategory);
                    });
                }
            });
        }

        public void SendInboxCountUpdateSignal(string sFKiOrgID, string s1ClickID, string sNodeID)
        {
            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            List<string> connectionIds = _orgConnections.GetAllConnectionsInOrg(sFKiOrgID);
            connectionIds.ForEach(sConnectionId => {
                LeadContext.Clients.Client(sConnectionId).InboxCountResult(s1ClickID, sNodeID);
            });
        }

        public void SendLeadUpdateSignal(dynamic oSignalR, string sOrgID)
        {
            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            List<string> connectionIds = _orgConnections.GetAllConnectionsInOrg(sOrgID);
            connectionIds.ForEach(sConnectionId => {
                LeadContext.Clients.Client(sConnectionId).ListRefresh(oSignalR);
            });
        }

        public void SendNewLeadSignal(dynamic oSignalR, string sOrgID)
        {
            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            List<string> connectionIds = _orgConnections.GetAllConnectionsInOrg(sOrgID);
            connectionIds.ForEach(sConnectionId => {
                LeadContext.Clients.Client(sConnectionId).ListAddRefresh(oSignalR);
            });
        }

        public void SendKPIUpdateSignal(string sOrgID, string sXIComponentID, string sGuid, List<CNV> oParam, string sKPIID)
        {
            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            List<string> connectionIds = _orgConnections.GetAllConnectionsInOrg(sOrgID);
            connectionIds.ForEach(sConnectionId => {
                LeadContext.Clients.Client(sConnectionId).KPIResult(sXIComponentID, sGuid, oParam, sKPIID);
            });
        
        }

        public void SendLeadTraceFlowChartSignal(string sOrgID, List<CNV> oWhrParams, bool bFLag) 
        {
            IHubContext LeadContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            List<string> connectionIds = _orgConnections.GetAllConnectionsInOrg(sOrgID);
            connectionIds.ForEach(sConnectionId => {
                LeadContext.Clients.Client(sConnectionId).LeadtraceFlowChat(oWhrParams, bFLag);
            });
        }

        [Authorize]
        public override Task OnConnected()
       {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            string sUserName = Context.User.Identity.Name;

            //var identity = (ClaimsIdentity)Context.User.Identity;
            //string sOrgID = identity.Claims.Where(x => x.Type == "FKiOrgID").SingleOrDefault()?.Value;
            //var hidentity = (ClaimsIdentity)Context.Request.GetHttpContext().User.Identity;
            //sOrgID = hidentity.Claims.Where(x => x.Type == "FKiOrgID").SingleOrDefault()?.Value;

            // Cannot add using XICore due to circular dependency.

            //XID1Click o1click = new XID1Click();
            //o1click.Name = "XIAppUsers";
            //string sQuery = String.Empty;
            //o1click.Query = sQuery;
            //var oOneClick = o1click.OneClick_Execute(null, o1click);


            string sCoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDatabase"];
            cConnectionString oConString = new cConnectionString();
            string sConString = oConString.ConnectionString(sCoreDatabase);
            XIDBAPI Connection = new XIDBAPI(sConString);
            Dictionary<string, object> Params = new Dictionary<string, object>();
            Params["sUserName"] = sUserName;
            XIAppUser oUser = Connection.Select<XIAppUser>("XIAPPUsers_AU_T", Params).SingleOrDefault();


            string sOrgID = oUser.FKiOrgID.ToString();
            string sRoleID = oUser.FKiRoleID.ToString();
            _orgConnections.Add(sOrgID, sRoleID, Context.ConnectionId);
            _connections.Add(sUserName, Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnDisconnected( bool bISDisconnect)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            string sUserName = Context.User.Identity.Name;
            string sOrgID = string.Empty;
            string sRoleID = string.Empty;

            string sCoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDatabase"];
            cConnectionString oConString = new cConnectionString();
            string sConString = oConString.ConnectionString(sCoreDatabase);
            XIDBAPI Connection = new XIDBAPI(sConString);
            Dictionary<string, object> Params = new Dictionary<string, object>();
            Params["sUserName"] = sUserName;
            XIAppUser oUser = Connection.Select<XIAppUser>("XIAPPUsers_AU_T", Params).SingleOrDefault();


            sOrgID = oUser.FKiOrgID.ToString();
            sRoleID = oUser.FKiRoleID.ToString();

            _orgConnections.Remove(sOrgID, sRoleID, Context.ConnectionId);
            _connections.Remove(sUserName, Context.ConnectionId);
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

        public async Task ReceivePing(string message)
        {
            
        }
    }
}