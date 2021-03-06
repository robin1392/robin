#if UNITY_EDITOR || UNITY_STANDALONE_LINUX
using System.Collections.Generic;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace MirageTest.Aws
{
    public class SQSService : MonoBehaviour
    {
        string _queueName = string.Empty;
        string _roleArn = string.Empty;
        string _roleName = string.Empty;

        private void Start()
        {
            //aws sts assume-role --role-arn "arn:aws:iam::153269277707:role/randomwars-gameliftfleetrole --role-session-name RandomWarsSession"
            _queueName = "randomdicewars-match-result";
            _roleArn = "arn:aws:iam::153269277707:role/randomwars-gameliftfleetrole";
            _roleName = "RandomWarsSession";
        }

        public async UniTask SendMessage(string matchResultJson)
        {
            AssumeRoleRequest roleRequest = new AssumeRoleRequest();
            roleRequest.RoleArn = _roleArn;
            roleRequest.RoleSessionName = _roleName;
            AmazonSecurityTokenServiceClient stsClient = new AmazonSecurityTokenServiceClient();
            var roleResponse = stsClient.AssumeRoleAsync(roleRequest);

            await roleResponse;

            if (roleResponse.Result == null)
            {
                Debug.LogError($"roleResponse 가 널입니다.");
                return;
            }

            IAmazonSQS sqs = new AmazonSQSClient(roleResponse.Result.Credentials);

            var getQueueUrlResponse = sqs.GetQueueUrl(_queueName);
            var myQueueUrl = getQueueUrlResponse.QueueUrl;
            var sqsMessageRequest = new SendMessageRequest(myQueueUrl, matchResultJson);
            var sendMessageResponse = sqs.SendMessageAsync(sqsMessageRequest);
            if (sendMessageResponse.Result == null)
            {
                Debug.LogError($"sendMessageResponse 가 null입니다.");
                return;
            }

            Debug.Log($"게임 결과 SQS 메세지 아이디: {sendMessageResponse.Result.MessageId}");
        }

        public async UniTask SendMessage2(string matchResultJson)
        {
            AssumeRoleRequest roleRequest = new AssumeRoleRequest();
            roleRequest.RoleArn = _roleArn;
            roleRequest.RoleSessionName = _roleName;
            AmazonSecurityTokenServiceClient stsClient = new AmazonSecurityTokenServiceClient();
            var roleResponse = stsClient.AssumeRoleAsync(roleRequest);

            await roleResponse;

            if (roleResponse.Result == null)
            {
                Debug.LogError($"roleResponse 가 널입니다.");
                return;
            }

            IAmazonSQS sqs = new AmazonSQSClient(roleResponse.Result.Credentials);

            var getQueueUrlResponse = sqs.GetQueueUrl(_queueName);
            var myQueueUrl = getQueueUrlResponse.QueueUrl;
            var sqsMessageRequest = new SendMessageRequest(myQueueUrl, matchResultJson);
            var sendMessageResponse = sqs.SendMessageAsync(sqsMessageRequest);
            if (sendMessageResponse.Result == null)
            {
                Debug.LogError($"sendMessageResponse 가 null입니다.");
                return;
            }

            Debug.Log($"메세지 아이디: {sendMessageResponse.Result.MessageId}");
        }
    }
}
#endif