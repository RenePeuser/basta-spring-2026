# Deletion Strategy
```csharp
public sealed class DeletionStrategy(IDependencyGraphResolver graphResolver,
                                     IDeletionPlanner deletionPlanner,
                                     IDeletionPlanExecutor deletionExecutor)
    : IDeletionOrchestrationStrategy
{
    public async Task<DeletionPlan> ExecuteAsync(DeleteDependencyRequest request,
                                                 CancellationToken cancellationToken)
    {
        // STEP 1 — Resolve Graph
        var graph = await graphResolver.ResolveAsync(request.RootIds, cancellationToken).ConfigureAwait(false);

        // STEP 2 — Create Plan
        var deletionPlan = deletionPlanner.CreatePlan(graph);

        // STEP 3 — Execute Plan
        await deletionExecutor.ExecuteAsync(deletionPlan, cancellationToken).ConfigureAwait(false);

        return plan;
    }
}
```

# Specific Deletion
```csharp
public interface IDeleteGraphItem
{
    bool CanHandle(GraphItem item);

    Task DeleteAsync(GraphItem item,
                     CancellationToken cancellationToken);
}


public sealed class DeleteAwsBedrockModel : IDeleteCapability
{
    public bool CanHandle(CapabilityType type)
        => type == CapabilityType.AwsBedrockModel;

    public Task DeleteAsync(DeletionNode node,
                            CancellationToken cancellationToken)
    {
        Console.WriteLine($"Deleting Bedrock Model {node.Id}");
        return Task.CompletedTask;
    }
}
```




# 🧪 Canvas Report

## 📋 Overview

| Affected Element | Type | Stage | ID |
|------------------|------|-------|-----|
| alex-test-fargate | AWS_AGENT_FARGATE | Workbench | `a9a8bde0-42d9-43a0-905d-5d5831b7e2e7` |


## 📊 Canvas Changes Diagram

```mermaid
flowchart LR

%% =============================
%% PROVIDER STYLES
%% =============================

classDef awsCompute fill:#1f2937,stroke:#334155,color:#e5e7eb,stroke-width:1.5px,rx:18px,ry:18px;
classDef awsStorage fill:#1e293b,stroke:#3b475c,color:#dbe3ec,stroke-width:1.5px,rx:18px,ry:18px;
classDef awsData fill:#242a35,stroke:#3f4a5a,color:#e2e8f0,stroke-width:1.5px,rx:18px,ry:18px;
classDef awsAI fill:#252f45,stroke:#3f4f6b,color:#e2e8f0,stroke-width:1.5px,rx:18px,ry:18px;

classDef gcpAI fill:#1c3434,stroke:#2f6666,color:#d1f3f3,stroke-width:1.5px,rx:18px,ry:18px;

classDef repo fill:#1b2430,stroke:#3b475c,color:#cbd5e1,stroke-width:1.5px,rx:18px,ry:18px;
classDef workspace fill:#26213a,stroke:#4a3f66,color:#ddd6fe,stroke-width:1.5px,rx:18px,ry:18px;

%% =============================
%% STATUS
%% =============================

classDef markedForDeletion fill:#2f2615,stroke:#f59e0b,color:#fef3c7,stroke-width:2px,rx:18px,ry:18px;
classDef added fill:#103a22,stroke:#22c55e,color:#d1fae5,stroke-width:2px,rx:18px,ry:18px;
classDef deleted fill:#3f1d1d,stroke:#ef4444,color:#fecaca,stroke-width:2px,rx:18px,ry:18px;
classDef changed fill:#3a2a10,stroke:#f59e0b,color:#fef3c7,stroke-width:2px,rx:18px,ry:18px;

%% Edge Text Styles
classDef edgeText fill:transparent,stroke:transparent,color:#94a3b8,font-weight:500;
classDef edgeTextDeleted fill:transparent,stroke:transparent,color:#f59e0b,font-weight:600;
classDef edgeTextAdded fill:transparent,stroke:transparent,color:#22c55e,font-weight:600;
classDef edgeTextChanged fill:transparent,stroke:transparent,color:#f59e0b,font-weight:600;

%% =============================
%% NODES
%% =============================

N0["Agent on Fargate<br/>AWS_AGENT_FARGATE"]:::awsCompute
N1["alex-test-bedrock<br/>AWS_BEDROCK_MODELS"]:::awsAI
N2["google-search-agent<br/>AWS_AGENT_FARGATE"]:::awsCompute
N3["my-new-s3-bucket<br/>AWS_S3_BUCKET"]:::awsStorage
N4["mz-models<br/>AWS_BEDROCK_MODELS"]:::awsAI
N5["postgresTest<br/>AWS_AURORA_SERVERLESS_V2_POSTGRES"]:::awsData
N6["mz-models-2<br/>AWS_BEDROCK_MODELS"]:::awsAI
N7["my-models<br/>AWS_BEDROCK_MODELS"]:::awsAI
N8["sdc-blueprint<br/>AWS_AGENT_FARGATE"]:::awsCompute
N9["mz-agent-2<br/>AWS_AGENT_FARGATE"]:::awsCompute
N10["mz-fargate<br/>AWS_AGENT_FARGATE"]:::awsCompute
N11["slack-agent<br/>AWS_AGENT_FARGATE"]:::awsCompute
N12["mz-bucket-2<br/>AWS_S3_BUCKET"]:::awsStorage
N13["alex-test-s3<br/>AWS_S3_BUCKET"]:::awsStorage
N14["alex-test-fargate<br/>AWS_AGENT_FARGATE"]:::markedForDeletion
N15["postgres<br/>AWS_AURORA_SERVERLESS_V2_POSTGRES"]:::awsData
N16["google-search-agent-repo<br/>GITLAB_IMPORTED_REPOSITORY"]:::repo
N17["google-search-agent-repo<br/>GITLAB_IMPORTED_REPOSITORY"]:::repo
N18["slack-agent-repo<br/>GITLAB_IMPORTED_REPOSITORY"]:::repo
N19["base-google-adk-agent<br/>SGPT_WORKSPACE"]:::workspace
N20["Base<br/>GCP_VERTEX_AI_MODELS"]:::gcpAI

%% =============================
%% RELATIONS (WITH TEXT NODES)
%% =============================

N2 --> L1 --> N20
L1["AWS_AGENT_FARGATE_USE_GCP_VERTEX_AI_MODELS"]:::edgeText

N14 --> L2 --> N13
L2["AWS_AGENT_FARGATE_READ_FROM_S3"]:::edgeTextDeleted

N19 --> L3 --> N8
L3["SGPT_TO_AWS_ADK_AGENT_FARGATE"]:::edgeText

N8 --> L4 --> N3
L4["AWS_AGENT_FARGATE_READ_FROM_S3"]:::edgeText

N11 --> L5 --> N7
L5["AWS_AGENT_FARGATE_USE_BEDROCK"]:::edgeText

N11 --> L6 --> N18
L6["AWS_AGENT_FARGATE_USE_GITLAB_IMPORTED_REPOSITORY"]:::edgeText

N2 --> L7 --> N5
L7["AWS_AGENT_FARGATE_READ_WRITE_TO_AURORA_POSTGRES"]:::edgeText

N2 --> L8 --> N17
L8["AWS_AGENT_FARGATE_USE_GITLAB_IMPORTED_REPOSITORY"]:::edgeText

N9 --> L9 --> N12
L9["AWS_AGENT_FARGATE_READ_FROM_S3"]:::edgeText

N9 --> L10 --> N6
L10["AWS_AGENT_FARGATE_USE_BEDROCK"]:::edgeText

N19 --> L11 --> N2
L11["SGPT_TO_AWS_ADK_AGENT_FARGATE"]:::edgeText

N2 --> L12 --> N15
L12["AWS_AGENT_FARGATE_READ_WRITE_TO_AURORA_POSTGRES"]:::edgeText

N19 --> L13 --> N14
L13["SGPT_TO_AWS_ADK_AGENT_FARGATE"]:::edgeTextDeleted

N14 --> L14 --> N1
L14["AWS_AGENT_FARGATE_USE_BEDROCK"]:::edgeTextDeleted

N10 --> L15 --> N4
L15["AWS_AGENT_FARGATE_USE_BEDROCK"]:::edgeText

%% =============================
%% EDGE COLORING
%% =============================

linkStyle 0 stroke:#3b475c,stroke-width:1.5px
linkStyle 1 stroke:#3b475c,stroke-width:1.5px
linkStyle 2 stroke:#f59e0b,stroke-width:2px
linkStyle 3 stroke:#f59e0b,stroke-width:2px
linkStyle 4 stroke:#3b475c,stroke-width:1.5px
linkStyle 5 stroke:#3b475c,stroke-width:1.5px
linkStyle 6 stroke:#3b475c,stroke-width:1.5px
linkStyle 7 stroke:#3b475c,stroke-width:1.5px
linkStyle 8 stroke:#3b475c,stroke-width:1.5px
linkStyle 9 stroke:#3b475c,stroke-width:1.5px
linkStyle 10 stroke:#3b475c,stroke-width:1.5px
linkStyle 11 stroke:#3b475c,stroke-width:1.5px
linkStyle 12 stroke:#3b475c,stroke-width:1.5px
linkStyle 13 stroke:#3b475c,stroke-width:1.5px
linkStyle 14 stroke:#3b475c,stroke-width:1.5px
linkStyle 15 stroke:#3b475c,stroke-width:1.5px
linkStyle 16 stroke:#3b475c,stroke-width:1.5px
linkStyle 17 stroke:#3b475c,stroke-width:1.5px
linkStyle 18 stroke:#3b475c,stroke-width:1.5px
linkStyle 19 stroke:#3b475c,stroke-width:1.5px
linkStyle 20 stroke:#3b475c,stroke-width:1.5px
linkStyle 21 stroke:#3b475c,stroke-width:1.5px
linkStyle 22 stroke:#3b475c,stroke-width:1.5px
linkStyle 23 stroke:#3b475c,stroke-width:1.5px
linkStyle 24 stroke:#f59e0b,stroke-width:2px
linkStyle 25 stroke:#f59e0b,stroke-width:2px
linkStyle 26 stroke:#f59e0b,stroke-width:2px
linkStyle 27 stroke:#f59e0b,stroke-width:2px
linkStyle 28 stroke:#3b475c,stroke-width:1.5px
linkStyle 29 stroke:#3b475c,stroke-width:1.5px
```


## 📊 Impact Summary

| Entity | Affected | Marked | Deleted |
|--------|----------|--------|---------|
| Capabilities | 1 | 1 | 0 |
| Relations | 3 | 3 | 0 |
| Nodes | 1 | 1 | 0 |
| Edges | 3 | 3 | 0 |

## 🟡 Affected Capabilities (1)

| Name | Type | ID |
|------|------|----|
| alex-test-fargate | AWS_AGENT_FARGATE | `a9a8bde0...` |

## 🟡 Affected Relations (3)

| Type | Source → Target |
|------|-----------------|
| AWS_AGENT_FARGATE_READ_FROM_S3 | alex-test-fargate → alex-test-s3 |
| AWS_AGENT_FARGATE_USE_BEDROCK | alex-test-fargate → alex-test-bedrock |
| SGPT_TO_AWS_ADK_AGENT_FARGATE | base-google-adk-agent → alex-test-fargate |

## 🟡 Affected Nodes (1)

| Capability | Type |
|------------|------|
| alex-test-fargate | AWS_AGENT_FARGATE |

## 🟡 Affected Edges (3)

| Type | Source → Target |
|------|-----------------|
| AWS_AGENT_FARGATE_READ_FROM_S3 | alex-test-fargate → alex-test-s3 |
| SGPT_TO_AWS_ADK_AGENT_FARGATE | base-google-adk-agent → alex-test-fargate |
| AWS_AGENT_FARGATE_USE_BEDROCK | alex-test-fargate → alex-test-bedrock |


## 📈 Statistics

- **Capabilities**: 25 → 25
- **Relations**: 15 → 15
- **Nodes**: 21 → 21 (0 added, 0 deleted, 1 changed, 1 marked)
- **Edges**: 15 → 15 (0 added, 0 deleted, 3 changed, 3 marked)

## 🎨 Color Legend

- 🟢 **Green** - Added (new entities)
- 🟡 **Yellow** - Marked for Deletion (will be deleted on next deploy)
- 🟠 **Orange** - Changed (modified entities)
- 🔴 **Red** - Deleted (no longer exists)
- ⚪ **White** - Unchanged
