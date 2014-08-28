﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace FastDFS.Client
{
    /// <summary>
    /// set metat data from storage server
    /// 
    /// Reqeust 
    ///     Cmd: STORAGE_PROTO_CMD_SET_METADATA 13
    ///     Body:
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename length 
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: meta data size 
    ///     @ 1 bytes: operation flag,  
    ///         'O' for overwrite all old metadata 
    ///         'M' for merge, insert when the meta item not exist, otherwise update it
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name 
    ///     @ filename bytes: filename
    ///     @ meta data bytes: each meta data seperated by \x01,
    ///         name and value seperated by \x02
    /// Response
    ///     Cmd: STORAGE_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///         
    /// </summary>
    public class SET_METADATA : FDFSRequest
    {
        private static SET_METADATA _instance = null;
        public static SET_METADATA Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SET_METADATA();
                return _instance;
            }
        }

        private SET_METADATA()
        {
        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            IPEndPoint endPoint = (IPEndPoint)paramList[0];
            string groupName = (string)paramList[1];
            string fileName = (string)paramList[2];
            IDictionary<string, string> metaData = (IDictionary<string, string>)paramList[3];
            string option = (MetaDataOption)paramList[4] == MetaDataOption.Overwrite ? "O" : "M";

            byte[] optionBuffer = Util.StringToByte(option);
            byte[] groupNameBuffer = Util.CreateGroupNameBuffer(groupName);
            byte[] fileNameBuffer = Util.StringToByte(fileName);
            byte[] metaDataBuffer = CreateMetaDataBuffer(metaData);

            byte[] fileNameLengthBuffer = Util.LongToBuffer(fileNameBuffer.Length);
            byte[] metaDataSizeBuffer = Util.LongToBuffer(metaDataBuffer.Length);

            int length = Consts.FDFS_PROTO_PKG_LEN_SIZE +  // filename length
                         Consts.FDFS_PROTO_PKG_LEN_SIZE +  // metadata size
                         1 +  // operation flag
                         Consts.FDFS_GROUP_NAME_MAX_LEN +  // group name
                         fileNameBuffer.Length +  // file name
                         metaDataBuffer.Length;            // metadata 

            List<byte> bodyBuffer = new List<byte>();
            bodyBuffer.AddRange(fileNameLengthBuffer);
            bodyBuffer.AddRange(metaDataSizeBuffer);
            bodyBuffer.AddRange(optionBuffer);
            bodyBuffer.AddRange(groupNameBuffer);
            bodyBuffer.AddRange(fileNameBuffer);
            bodyBuffer.AddRange(metaDataBuffer);

            Debug.Assert(length == bodyBuffer.Count);

            SET_METADATA request = new SET_METADATA();
            request.Connection = ConnectionManager.GetStorageConnection(endPoint);
            request.Body = bodyBuffer.ToArray();
            request.Header = new FDFSHeader(bodyBuffer.Count, Consts.STORAGE_PROTO_CMD_SET_METADATA, 0);
            return request;
        }

        private byte[] CreateMetaDataBuffer(IDictionary<string, string> metaData)
        {
            //List<byte> metaDataBuffer = new List<byte>();
            //foreach (KeyValuePair<string, string> p in metaData)
            //{
            //    // insert a separater if this is not the first meta data item.
            //    if (metaDataBuffer.Count == 0)
            //    {
            //        metaDataBuffer.Add(Consts.METADATA_PAIR_SEPARATER);
            //    }

            //    metaDataBuffer.AddRange(Util.StringToByte(p.Key));
            //    metaDataBuffer.Add(Consts.METADATA_KEY_VALUE_SEPARATOR);
            //    metaDataBuffer.AddRange(Util.StringToByte(p.Value));
            //}
            //return metaDataBuffer.ToArray();

            NameValuePair[] nameValuePair = new NameValuePair[metaData.Count];
            int index = 0;
            foreach (var key in metaData.Keys)
            {
                nameValuePair[index] = new NameValuePair(key, metaData[key]);
                index++;
            }
            if (metaData.Count == 0)
            {
                return Util.StringToByte("");
            }

            StringBuilder sb = new StringBuilder(32 * metaData.Count);
            sb.Append(nameValuePair[0].Name).Append(Consts.FDFS_FIELD_SEPERATOR).Append(nameValuePair[0].Value);
            for (int i = 1; i < nameValuePair.Length; i++)
            {
                sb.Append(Consts.FDFS_RECORD_SEPERATOR);
                sb.Append(nameValuePair[i].Name).Append(Consts.FDFS_FIELD_SEPERATOR).Append(nameValuePair[i].Value);
            }

            return Util.StringToByte(sb.ToString());
        }
    }

    /// <summary>
    /// Metedata的结构
    /// </summary>
    public class NameValuePair
    {
        private string _name;
        private string _value;

        public NameValuePair()
        {
        }

        /// <summary>
        /// 初始化<see cref="NameValuePair"/> 对象.
        /// </summary>
        /// <param name="name">Metedata的名称.</param>
        public NameValuePair(string name)
        {
            _name = name;
        }

        /// <summary>
        /// 初始化<see cref="NameValuePair"/> 对象.
        /// </summary>
        /// <param name="name">Mmetedata的名称.</param>
        /// <param name="value">Metedata的值.</param>
        public NameValuePair(string name, string value)
        {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// 得到或者设置Metedata的名称
        /// </summary>
        /// <value>Metedata的名称.</value>
        public virtual string Name
        {
            get { return _name; }

            set { _name = value; }
        }

        /// <summary>
        /// 得到或者设置Metedata的值.
        /// </summary>
        /// <value>Metedata的值.</value>
        public virtual string Value
        {
            get { return _value; }

            set { _value = value; }
        }
    }
}
