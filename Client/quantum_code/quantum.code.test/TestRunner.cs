using System;
using System.IO;
using System.Threading;
using Photon.Deterministic;
using Quantum;

public class TestRunner
{
    private SessionContainer _sessionContainer;
    
    public TestRunner(string pathToLUT, string pathToDatabaseFile)
    {
        FPLut.Init(pathToLUT);

        FileLoader.Init(new DotNetFileLoader(Path.GetDirectoryName(pathToDatabaseFile)));

        var serializer = new QuantumJsonSerializer();
        
        SessionContainer container = new SessionContainer(new DeterministicSessionConfig()
        {
            PlayerCount = 1,
            
        }, new RuntimeConfig()
        {
            Map = new AssetRefMap(){Id = 1710471675203725834 },
            SimulationConfig = new AssetRefSimulationConfig(){ Id = 5}
        });

        var callbackDispatcher = new CallbackDispatcher();

        container.assetSerializer = serializer;
        container.resourceManager =
            new ResourceManagerStatic(serializer.DeserializeAssets(File.ReadAllBytes(pathToDatabaseFile)),
                container.allocator);
        container.callbackDispatcher = callbackDispatcher;
        container.Start();

        _sessionContainer = container;
    }
    
    public Frame Frame => _sessionContainer.game.Session.FrameVerified as Frame;

    public void Update(double? dt = null)
    {
        _sessionContainer.Service(0);
    }
}