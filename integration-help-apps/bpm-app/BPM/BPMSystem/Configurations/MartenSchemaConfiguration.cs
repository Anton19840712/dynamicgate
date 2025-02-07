using BPMEngine.DB.Consts;
using BPMIntegration.Models;
using Marten;

public class MartenSchemaConfiguration : MartenRegistry
{
    public static void Configure(StoreOptions options)
    {
        options.Connection(DBConsts.DBConnections.ConnectionString_BPM!);

        options.Schema.For<IntegrationEntity>()
            //.DocumentAlias("integration")
            .Duplicate(x => x.InQueueName!)
            .Duplicate(x => x.OutQueueName!)
            .Duplicate(x => x.IncomingModel!);

        options.Schema.For<OutboxMessage>()
            // .DocumentAlias("outbox")
            .Duplicate(x => x.EventType)
            .Duplicate(x => x.Payload)
            .Duplicate(x => x.IsProcessed)
            .Duplicate(x => x.OutQueueu)
            .Duplicate(x => x.CreatedAt);
    }
}
