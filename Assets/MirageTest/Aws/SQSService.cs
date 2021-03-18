using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MirageTest.Scripts;
using UnityEngine;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
//using Amazon.SecurityToken.Model;
//using Amazon.SecurityToken;
using Newtonsoft.Json;

namespace MirageTest.Aws
{
    public class SQSService : MonoBehaviour
    {
        string _queueName = string.Empty;
        string _roleArn = string.Empty;
        string _roleName = string.Empty;


        private void Start()
        {
            _queueName = "randomdicewars-match-result";
            _roleArn = "arn:aws:iam::153269277707:role/randomwars-gameliftfleetrole";
            _roleName = "RandomWarsSession";
        }


        public bool SendMessage(List<UserMatchResult> listMatchResult)
        {
            //AssumeRoleRequest roleRequest = new AssumeRoleRequest();
            //roleRequest.RoleArn = _roleArn;
            //roleRequest.RoleSessionName = _roleName;
            //AmazonSecurityTokenServiceClient stsClient = new AmazonSecurityTokenServiceClient();
            //AssumeRoleResponse roleResponse = stsClient.AssumeRole(roleRequest);
            //if (roleResponse == null)
            //{
            //    return false;
            //}

            //// db 클라이언트 생성
            //IAmazonSQS sqs = new AmazonSQSClient(roleResponse.Credentials);
            IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.APNortheast2);

            var getQueueUrlResponse = sqs.GetQueueUrl(_queueName);
            var myQueueUrl = getQueueUrlResponse.QueueUrl;
            var sqsMessageRequest = new SendMessageRequest(myQueueUrl, JsonConvert.SerializeObject(listMatchResult));
            var sendMessageResponse = sqs.SendMessage(sqsMessageRequest);
            if (sendMessageResponse == null)
            {
                return false;
            }

            return true;
        }
    }
}
