using Mirage;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class NetworkBehaviourParentSetter : NetworkBehaviour
    {
        protected virtual string ParentName { get; } = null;

        private void Awake()
        {
            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStartServer.AddListener(StartServer);
        }

        private void StartServer()
        {
            var parent = GetOrCreateActorParent(Server.transform);
            ChangeParent(parent);
        }

        private void StartClient()
        {
            var parent = GetOrCreateActorParent(Client.transform);
            ChangeParent(parent);
        }

        Transform GetOrCreateActorParent(Transform root)
        {
            var parent = root.Find(ParentName);
            if (parent != null)
            {
                return parent;
            }

            parent = new GameObject(ParentName).transform;
            parent.parent = root;
            return parent;
        }

        void ChangeParent(Transform root)
        {
            gameObject.transform.SetParent(root, false);
        }
    }
}
