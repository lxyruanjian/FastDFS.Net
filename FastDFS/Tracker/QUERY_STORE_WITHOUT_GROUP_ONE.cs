using System;
using System.Collections.Generic;
using System.Text;

namespace FastDFS.Client
{
    /// <summary>
    /// query which storage server to store file
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE 101
    ///     Body: 
    ///     
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    ///     @ 1 byte: store path index on the storage server
    /// </summary>
    public class QUERY_STORE_WITHOUT_GROUP_ONE : FDFSRequest
    {
        private static QUERY_STORE_WITHOUT_GROUP_ONE _instance = null;
        public static QUERY_STORE_WITHOUT_GROUP_ONE Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QUERY_STORE_WITHOUT_GROUP_ONE();
                return _instance;
            }
        }
        private QUERY_STORE_WITHOUT_GROUP_ONE()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramList">
        /// 1,string groupName-->the storage groupName
        /// </param>
        /// <returns></returns>
        public override FDFSRequest GetRequest(params object[] paramList)
        {
            QUERY_STORE_WITHOUT_GROUP_ONE result = new QUERY_STORE_WITHOUT_GROUP_ONE();

            result.Header = new FDFSHeader(0,
                Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITHOUT_GROUP_ONE, 0);
            return result;
        }
    }
}
