#if UNITY_EDITOR || UNITY_STANDALONE
using System.Collections.Generic;
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
            _queueName = "randomdicewars-match-result";
            _roleArn = "arn:aws:iam::153269277707:role/randomwars-gameliftfleetrole";
            _roleName = "RandomWarsSession";
        }

        public async UniTask SendMessage(List<UserMatchResult> listMatchResult)
        {
            // AssumeRoleRequest roleRequest = new AssumeRoleRequest();
            // roleRequest.RoleArn = _roleArn;
            // roleRequest.RoleSessionName = _roleName;
            // AmazonSecurityTokenServiceClient stsClient = new AmazonSecurityTokenServiceClient();
            // var roleResponse = stsClient.AssumeRoleAsync(roleRequest);
            //
            // await roleResponse;
            //
            // if (roleResponse.Result == null)
            // {
            //     Debug.LogError($"roleResponse 가 널입니다.");
            //     return;
            // }
            //
            // IAmazonSQS sqs = new AmazonSQSClient(roleResponse.Result.Credentials);
            //
            // var getQueueUrlResponse = sqs.GetQueueUrlAsync(_queueName);
            // await getQueueUrlResponse;
            // if (getQueueUrlResponse.Result == null)
            // {
            //     Debug.LogError($"getQueueUrlResponse 실패");
            //     return;
            // }
            //
            // var myQueueUrl = getQueueUrlResponse.Result.QueueUrl;
            // var sqsMessageRequest = new SendMessageRequest(myQueueUrl, JsonConvert.SerializeObject(listMatchResult));
            // var sendMessageResponse = sqs.SendMessageAsync(sqsMessageRequest);
            // if (sendMessageResponse.Result == null)
            // {
            //     Debug.LogError($"sendMessageResponse 실패");
            //     return;
            // }
            //
            // Debug.Log($"메세지 아이디: {sendMessageResponse.Result.MessageId}");
        }
    }
}
#endif