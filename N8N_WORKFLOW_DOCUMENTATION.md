# Plasma Siege N8N Workflow Documentation

## Overview

This document provides comprehensive setup instructions and workflow templates for integrating n8n automation with Plasma Siege. All workflows run locally in Docker and handle match results, token distribution, and NFT minting.

## Local N8N Setup

### 1. Docker Installation

```bash
# Install Docker on Pop!_OS / Ubuntu
sudo apt update
sudo apt install docker.io docker-compose
sudo usermod -aG docker $USER
# Log out and back in for group changes to take effect
```

### 2. N8N Container Setup

```bash
# Create n8n data directory
mkdir -p ~/.n8n

# Run n8n container with persistent storage
docker run -d \
  --name n8n-plasmasiege \
  --restart unless-stopped \
  -p 5678:5678 \
  -e N8N_BASIC_AUTH_ACTIVE=true \
  -e N8N_BASIC_AUTH_USER=admin \
  -e N8N_BASIC_AUTH_PASSWORD=plasmasiege2024 \
  -e N8N_ENCRYPTION_KEY=$(openssl rand -hex 32) \
  -e WEBHOOK_URL=http://localhost:5678 \
  -v ~/.n8n:/home/node/.n8n \
  n8nio/n8n

# Verify n8n is running
docker ps | grep n8n-plasmasiege

# Access n8n at http://localhost:5678
# Login with admin / plasmasiege2024
```

### 3. Environment Configuration

Create `.env` file in n8n data directory:

```bash
cat > ~/.n8n/.env << 'EOF'
# Supabase
SUPABASE_URL=https://xxxxx.supabase.co
SUPABASE_ANON_KEY=eyJhbGc...
SUPABASE_SERVICE_KEY=eyJhbGc...

# Solana
SOLANA_RPC_URL=https://api.devnet.solana.com
PLASMA_TOKEN_MINT=YOUR_TOKEN_MINT_ADDRESS
SHIP_COLLECTION_ADDRESS=YOUR_NFT_COLLECTION_ADDRESS
TREASURY_WALLET_PRIVATE_KEY=YOUR_TREASURY_KEY

# Unity Webhook Secret
WEBHOOK_SECRET=your-secure-webhook-secret
EOF
```

## Core Workflows

### 1. Match Complete Workflow

**Purpose:** Process match results and distribute PLASMA token rewards

**Webhook URL:** `http://localhost:5678/webhook/match-complete`

```json
{
  "name": "Match Complete - Token Distribution",
  "nodes": [
    {
      "parameters": {
        "httpMethod": "POST",
        "path": "match-complete",
        "responseMode": "onReceived",
        "responseData": "allEntries",
        "options": {}
      },
      "id": "webhook_trigger",
      "name": "Webhook",
      "type": "n8n-nodes-base.webhook",
      "typeVersion": 1,
      "position": [250, 300],
      "webhookId": "match-complete"
    },
    {
      "parameters": {
        "conditions": {
          "string": [
            {
              "value1": "={{$json[\"headers\"][\"x-webhook-secret\"]}}",
              "value2": "={{$env[\"WEBHOOK_SECRET\"]}}"
            }
          ]
        }
      },
      "id": "verify_secret",
      "name": "Verify Secret",
      "type": "n8n-nodes-base.if",
      "typeVersion": 1,
      "position": [450, 300]
    },
    {
      "parameters": {
        "functionCode": "// Validate match data\nconst matchData = items[0].json.body;\n\n// Required fields\nconst required = ['matchId', 'winnerId', 'loserId', 'winnerWallet', 'loserWallet', 'plasmaCollected', 'matchChecksum'];\n\nfor (const field of required) {\n  if (!matchData[field]) {\n    throw new Error(`Missing required field: ${field}`);\n  }\n}\n\n// Calculate rewards\nconst entryFee = 10;\nconst winBonus = 20;\nconst totalReward = winBonus + parseFloat(matchData.plasmaCollected);\n\n// Add calculated values\nmatchData.entryFee = entryFee;\nmatchData.winBonus = winBonus;\nmatchData.totalReward = totalReward;\nmatchData.timestamp = new Date().toISOString();\n\nreturn [{json: matchData}];"
      },
      "id": "validate_calculate",
      "name": "Validate & Calculate",
      "type": "n8n-nodes-base.function",
      "typeVersion": 1,
      "position": [650, 300]
    },
    {
      "parameters": {
        "resource": "database",
        "operation": "insert",
        "schema": "public",
        "table": "match_results",
        "columns": "matchId,winnerId,loserId,entryFee,winnerReward,plasmaCollected,matchChecksum,createdAt",
        "additionalFields": {}
      },
      "id": "save_match",
      "name": "Save Match Result",
      "type": "n8n-nodes-base.supabase",
      "typeVersion": 1,
      "position": [850, 300],
      "credentials": {
        "supabaseApi": {
          "id": "1",
          "name": "Supabase"
        }
      }
    },
    {
      "parameters": {
        "resource": "database",
        "operation": "update",
        "schema": "public",
        "table": "player_stats",
        "updateKey": "user_id",
        "columns": "plasma_balance",
        "additionalFields": {
          "plasma_balance": "={{$json[\"currentBalance\"] + $json[\"totalReward\"]}}"
        }
      },
      "id": "update_winner_balance",
      "name": "Update Winner Balance",
      "type": "n8n-nodes-base.supabase",
      "typeVersion": 1,
      "position": [1050, 200],
      "credentials": {
        "supabaseApi": {
          "id": "1",
          "name": "Supabase"
        }
      }
    },
    {
      "parameters": {
        "method": "POST",
        "url": "={{$env[\"SOLANA_RPC_URL\"]}}",
        "sendHeaders": true,
        "headerParameters": {
          "parameter": [
            {
              "name": "Content-Type",
              "value": "application/json"
            }
          ]
        },
        "sendBody": true,
        "bodyParameters": {
          "parameter": [
            {
              "name": "jsonrpc",
              "value": "2.0"
            },
            {
              "name": "method",
              "value": "sendTransaction"
            },
            {
              "name": "params",
              "value": "={{$json[\"serializedTransaction\"]}}"
            }
          ]
        }
      },
      "id": "blockchain_transfer",
      "name": "Transfer PLASMA Tokens",
      "type": "n8n-nodes-base.httpRequest",
      "typeVersion": 3,
      "position": [1250, 300]
    },
    {
      "parameters": {
        "values": {
          "string": [
            {
              "name": "status",
              "value": "success"
            },
            {
              "name": "matchId",
              "value": "={{$node[\"webhook_trigger\"].json[\"body\"][\"matchId\"]}}"
            },
            {
              "name": "reward",
              "value": "={{$node[\"validate_calculate\"].json[\"totalReward\"]}}"
            }
          ]
        }
      },
      "id": "success_response",
      "name": "Success Response",
      "type": "n8n-nodes-base.set",
      "typeVersion": 1,
      "position": [1450, 300]
    }
  ],
  "connections": {
    "webhook_trigger": {
      "main": [[{"node": "verify_secret", "type": "main", "index": 0}]]
    },
    "verify_secret": {
      "main": [
        [{"node": "validate_calculate", "type": "main", "index": 0}],
        [{"node": "error_response", "type": "main", "index": 0}]
      ]
    },
    "validate_calculate": {
      "main": [[{"node": "save_match", "type": "main", "index": 0}]]
    },
    "save_match": {
      "main": [[
        {"node": "update_winner_balance", "type": "main", "index": 0},
        {"node": "create_plasma_transaction", "type": "main", "index": 0}
      ]]
    },
    "update_winner_balance": {
      "main": [[{"node": "blockchain_transfer", "type": "main", "index": 0}]]
    },
    "blockchain_transfer": {
      "main": [[{"node": "success_response", "type": "main", "index": 0}]]
    }
  }
}
```

### 2. NFT Mint Request Workflow

**Purpose:** Queue and process ship NFT minting when ships reach level 10

**Webhook URL:** `http://localhost:5678/webhook/nft-mint-request`

```json
{
  "name": "NFT Mint Request - Ship NFT Creation",
  "nodes": [
    {
      "parameters": {
        "httpMethod": "POST",
        "path": "nft-mint-request",
        "responseMode": "onReceived",
        "options": {}
      },
      "id": "webhook_trigger",
      "name": "Webhook",
      "type": "n8n-nodes-base.webhook",
      "typeVersion": 1,
      "position": [250, 300]
    },
    {
      "parameters": {
        "resource": "database",
        "operation": "get",
        "schema": "public",
        "table": "player_ships",
        "limit": 1,
        "filters": {
          "conditions": [
            {
              "field": "id",
              "condition": "equals",
              "value": "={{$json[\"body\"][\"shipId\"]}}"
            }
          ]
        }
      },
      "id": "get_ship_data",
      "name": "Get Ship Data",
      "type": "n8n-nodes-base.supabase",
      "typeVersion": 1,
      "position": [450, 300]
    },
    {
      "parameters": {
        "functionCode": "// Generate NFT metadata\nconst ship = items[0].json[0];\n\nif (ship.level < 10) {\n  throw new Error('Ship must be level 10 to mint as NFT');\n}\n\nif (ship.is_nft) {\n  throw new Error('Ship is already an NFT');\n}\n\n// Get ship upgrades\nconst upgrades = ship.upgrades || [];\n\n// Create metadata\nconst metadata = {\n  name: ship.ship_name || `${ship.ship_type.toUpperCase()} #${ship.id.slice(0, 8)}`,\n  symbol: 'PSS',\n  description: `Level ${ship.level} ${ship.ship_type} spaceship from Plasma Siege`,\n  image: `https://arweave.net/plasma-siege-ships/${ship.ship_type}-${ship.id}.png`,\n  attributes: [\n    { trait_type: 'Ship Type', value: ship.ship_type },\n    { trait_type: 'Level', value: ship.level },\n    { trait_type: 'Experience', value: ship.experience },\n    { trait_type: 'Fuel Upgrades', value: upgrades.filter(u => u.type === 'fuel').length },\n    { trait_type: 'Shield Upgrades', value: upgrades.filter(u => u.type === 'shield').length },\n    { trait_type: 'Weapon Upgrades', value: upgrades.filter(u => u.type === 'weapon').length }\n  ],\n  properties: {\n    files: [{\n      uri: `https://arweave.net/plasma-siege-ships/${ship.ship_type}-${ship.id}.png`,\n      type: 'image/png'\n    }],\n    category: 'image',\n    creators: [{\n      address: $env[\"TREASURY_WALLET\"],\n      share: 100\n    }]\n  }\n};\n\nreturn [{\n  json: {\n    shipId: ship.id,\n    userId: ship.user_id,\n    metadata: metadata,\n    mintCost: 100 // 100 PLASMA to mint\n  }\n}];"
      },
      "id": "generate_metadata",
      "name": "Generate NFT Metadata",
      "type": "n8n-nodes-base.function",
      "typeVersion": 1,
      "position": [650, 300]
    },
    {
      "parameters": {
        "resource": "database",
        "operation": "insert",
        "schema": "public",
        "table": "nft_mint_queue",
        "columns": "ship_id,user_id,metadata,mint_cost,status",
        "additionalFields": {}
      },
      "id": "queue_minting",
      "name": "Queue NFT Minting",
      "type": "n8n-nodes-base.supabase",
      "typeVersion": 1,
      "position": [850, 300]
    },
    {
      "parameters": {
        "method": "POST",
        "url": "https://api.metaplex.com/mint",
        "authentication": "genericCredentialType",
        "genericAuthType": "httpHeaderAuth",
        "sendHeaders": true,
        "headerParameters": {
          "parameter": [
            {
              "name": "Content-Type",
              "value": "application/json"
            }
          ]
        },
        "sendBody": true,
        "bodyParameters": {
          "parameter": [
            {
              "name": "metadata",
              "value": "={{$json[\"metadata\"]}}"
            },
            {
              "name": "collection",
              "value": "={{$env[\"SHIP_COLLECTION_ADDRESS\"]}}"
            }
          ]
        }
      },
      "id": "mint_nft",
      "name": "Mint NFT",
      "type": "n8n-nodes-base.httpRequest",
      "typeVersion": 3,
      "position": [1050, 300]
    },
    {
      "parameters": {
        "resource": "database",
        "operation": "update",
        "schema": "public",
        "table": "player_ships",
        "updateKey": "id",
        "columns": "is_nft,nft_mint_address,nft_metadata",
        "additionalFields": {
          "is_nft": true,
          "nft_mint_address": "={{$json[\"mintAddress\"]}}",
          "nft_metadata": "={{$json[\"metadata\"]}}"
        }
      },
      "id": "update_ship",
      "name": "Update Ship Record",
      "type": "n8n-nodes-base.supabase",
      "typeVersion": 1,
      "position": [1250, 300]
    }
  ]
}
```

### 3. Token Pickup Workflow

**Purpose:** Handle in-match token collection events

**Webhook URL:** `http://localhost:5678/webhook/token-pickup`

```json
{
  "name": "Token Pickup - In-Match Collection",
  "nodes": [
    {
      "parameters": {
        "httpMethod": "POST",
        "path": "token-pickup",
        "responseMode": "onReceived",
        "options": {}
      },
      "id": "webhook_trigger",
      "name": "Webhook",
      "type": "n8n-nodes-base.webhook",
      "typeVersion": 1,
      "position": [250, 300]
    },
    {
      "parameters": {
        "functionCode": "// Process token pickup\nconst pickup = items[0].json.body;\n\n// Validate data\nif (!pickup.playerId || !pickup.matchId || !pickup.tokenValue) {\n  throw new Error('Invalid token pickup data');\n}\n\n// Token values: 1-5 PLASMA\nif (pickup.tokenValue < 1 || pickup.tokenValue > 5) {\n  throw new Error('Invalid token value');\n}\n\n// Add timestamp\npickup.timestamp = new Date().toISOString();\npickup.eventType = 'token_pickup';\n\nreturn [{json: pickup}];"
      },
      "id": "validate_pickup",
      "name": "Validate Pickup",
      "type": "n8n-nodes-base.function",
      "typeVersion": 1,
      "position": [450, 300]
    },
    {
      "parameters": {
        "resource": "database",
        "operation": "insert",
        "schema": "public",
        "table": "match_events",
        "columns": "match_id,player_id,event_type,event_data,created_at",
        "additionalFields": {
          "event_data": {
            "tokenValue": "={{$json[\"tokenValue\"]}}",
            "position": "={{$json[\"position\"]}}"
          }
        }
      },
      "id": "log_event",
      "name": "Log Pickup Event",
      "type": "n8n-nodes-base.supabase",
      "typeVersion": 1,
      "position": [650, 300]
    }
  ]
}
```

### 4. Daily Rewards Workflow

**Purpose:** Automated daily login bonuses

**Cron Schedule:** `0 0 * * *` (Daily at midnight)

```json
{
  "name": "Daily Rewards - Login Bonus",
  "nodes": [
    {
      "parameters": {
        "triggerTimes": {
          "item": [{
            "mode": "everyDay",
            "hour": 0,
            "minute": 0
          }]
        }
      },
      "id": "cron_trigger",
      "name": "Daily Trigger",
      "type": "n8n-nodes-base.cron",
      "typeVersion": 1,
      "position": [250, 300]
    },
    {
      "parameters": {
        "resource": "database",
        "operation": "getAll",
        "schema": "public",
        "table": "users",
        "filters": {
          "conditions": [{
            "field": "last_login",
            "condition": "dateAfter",
            "value": "={{$today.minus({days: 1}).toISO()}}"
          }]
        }
      },
      "id": "get_active_users",
      "name": "Get Active Users",
      "type": "n8n-nodes-base.supabase",
      "typeVersion": 1,
      "position": [450, 300]
    },
    {
      "parameters": {
        "batchSize": 10,
        "options": {}
      },
      "id": "split_batches",
      "name": "Split Into Batches",
      "type": "n8n-nodes-base.splitInBatches",
      "typeVersion": 1,
      "position": [650, 300]
    },
    {
      "parameters": {
        "functionCode": "// Calculate daily reward\nconst users = items;\nconst processedUsers = [];\n\nfor (const item of users) {\n  const user = item.json;\n  \n  // Base reward: 10 PLASMA\n  let reward = 10;\n  \n  // Streak bonus\n  const streak = user.login_streak || 0;\n  if (streak >= 7) reward += 10;  // Week streak\n  if (streak >= 30) reward += 20; // Month streak\n  \n  processedUsers.push({\n    json: {\n      userId: user.id,\n      walletAddress: user.wallet_address,\n      reward: reward,\n      streak: streak + 1,\n      reason: 'daily_login_bonus'\n    }\n  });\n}\n\nreturn processedUsers;"
      },
      "id": "calculate_rewards",
      "name": "Calculate Rewards",
      "type": "n8n-nodes-base.function",
      "typeVersion": 1,
      "position": [850, 300]
    }
  ]
}
```

## Unity Integration

### 1. N8NWebhookManager.cs

Create this script in Unity to handle webhook communications:

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PlasmasieSiege.Blockchain
{
    public class N8NWebhookManager : MonoBehaviour
    {
        private static N8NWebhookManager instance;
        public static N8NWebhookManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("N8NWebhookManager");
                    instance = go.AddComponent<N8NWebhookManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private const string WEBHOOK_BASE_URL = "http://localhost:5678/webhook/";
        private const string WEBHOOK_SECRET = "your-secure-webhook-secret"; // Match n8n .env

        [Serializable]
        public class MatchResult
        {
            public string matchId;
            public string winnerId;
            public string loserId;
            public string winnerWallet;
            public string loserWallet;
            public float plasmaCollected;
            public string matchChecksum;
        }

        [Serializable]
        public class NFTMintRequest
        {
            public string shipId;
            public string userId;
            public string userWallet;
        }

        [Serializable]
        public class TokenPickup
        {
            public string playerId;
            public string matchId;
            public int tokenValue;
            public Vector3 position;
        }

        public void SendMatchResult(MatchResult result, Action<bool> callback = null)
        {
            StartCoroutine(SendWebhook("match-complete", result, callback));
        }

        public void SendNFTMintRequest(NFTMintRequest request, Action<bool> callback = null)
        {
            StartCoroutine(SendWebhook("nft-mint-request", request, callback));
        }

        public void SendTokenPickup(TokenPickup pickup, Action<bool> callback = null)
        {
            StartCoroutine(SendWebhook("token-pickup", pickup, callback));
        }

        private IEnumerator SendWebhook<T>(string endpoint, T data, Action<bool> callback)
        {
            string url = WEBHOOK_BASE_URL + endpoint;
            string jsonData = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("X-Webhook-Secret", WEBHOOK_SECRET);

                yield return request.SendWebRequest();

                bool success = request.result == UnityWebRequest.Result.Success;
                
                if (!success)
                {
                    Debug.LogError($"N8N Webhook Error: {request.error}");
                    // Implement retry logic here
                    yield return new WaitForSeconds(2f);
                    StartCoroutine(SendWebhook(endpoint, data, callback)); // Retry once
                }
                else
                {
                    Debug.Log($"N8N Webhook Success: {endpoint}");
                }

                callback?.Invoke(success);
            }
        }
    }
}
```

### 2. Example Usage in Unity

```csharp
// After match completes
void OnMatchComplete()
{
    var result = new N8NWebhookManager.MatchResult
    {
        matchId = currentMatch.id,
        winnerId = winner.userId,
        loserId = loser.userId,
        winnerWallet = winner.walletAddress,
        loserWallet = loser.walletAddress,
        plasmaCollected = totalPlasmaCollected,
        matchChecksum = CalculateMatchChecksum()
    };

    N8NWebhookManager.Instance.SendMatchResult(result, (success) =>
    {
        if (success)
        {
            ShowMatchResultsUI();
        }
        else
        {
            ShowErrorMessage("Failed to process match results");
        }
    });
}

// When ship reaches level 10
void OnShipLevelUp(Ship ship)
{
    if (ship.level >= 10 && !ship.isNFT)
    {
        ShowNFTMintButton();
    }
}

// Token pickup
void OnTokenPickup(Token token)
{
    var pickup = new N8NWebhookManager.TokenPickup
    {
        playerId = currentPlayer.id,
        matchId = currentMatch.id,
        tokenValue = token.value,
        position = token.transform.position
    };

    N8NWebhookManager.Instance.SendTokenPickup(pickup);
}
```

## Testing N8N Workflows

### 1. Test with cURL

```bash
# Test match complete webhook
curl -X POST http://localhost:5678/webhook/match-complete \
  -H "Content-Type: application/json" \
  -H "X-Webhook-Secret: your-secure-webhook-secret" \
  -d '{
    "matchId": "test-match-123",
    "winnerId": "user-456",
    "loserId": "user-789",
    "winnerWallet": "wallet1",
    "loserWallet": "wallet2",
    "plasmaCollected": 15.5,
    "matchChecksum": "abc123"
  }'

# Test NFT mint request
curl -X POST http://localhost:5678/webhook/nft-mint-request \
  -H "Content-Type: application/json" \
  -H "X-Webhook-Secret: your-secure-webhook-secret" \
  -d '{
    "shipId": "ship-123",
    "userId": "user-456",
    "userWallet": "wallet1"
  }'
```

### 2. Monitor N8N Logs

```bash
# View n8n container logs
docker logs -f n8n-plasmasiege

# Check workflow execution history in n8n UI
# http://localhost:5678/workflow/executions
```

### 3. Debug Common Issues

**Issue: Webhook not receiving data**
- Check Docker container is running: `docker ps`
- Verify port 5678 is accessible: `netstat -tlnp | grep 5678`
- Test webhook URL directly with cURL

**Issue: Supabase connection fails**
- Verify credentials in n8n environment
- Test Supabase connection in n8n credentials page
- Check RLS policies allow n8n service role

**Issue: Solana transactions fail**
- Ensure devnet has sufficient SOL
- Verify token mint addresses are correct
- Check treasury wallet has token authority

## Production Considerations

1. **Security**
   - Use strong webhook secrets
   - Implement rate limiting
   - Add request validation
   - Use HTTPS in production

2. **Reliability**
   - Implement retry logic with exponential backoff
   - Add error notifications (email/Discord)
   - Monitor webhook response times
   - Set up health checks

3. **Scaling**
   - Use n8n queue mode for high volume
   - Consider multiple n8n instances
   - Implement webhook queuing in Unity
   - Add caching where appropriate

4. **Monitoring**
   - Set up n8n metrics export
   - Create dashboards for webhook stats
   - Alert on failed workflows
   - Log all token transactions

## Backup and Migration

```bash
# Backup n8n data
docker exec n8n-plasmasiege n8n export:workflow --all --output=/home/node/backup.json
docker cp n8n-plasmasiege:/home/node/backup.json ./n8n-backup-$(date +%Y%m%d).json

# Restore n8n data
docker cp ./n8n-backup.json n8n-plasmasiege:/home/node/backup.json
docker exec n8n-plasmasiege n8n import:workflow --input=/home/node/backup.json

# Migrate to new server
# 1. Export workflows as above
# 2. Copy ~/.n8n directory
# 3. Import on new server
```

---

This documentation provides a complete guide for integrating n8n with Plasma Siege, from local setup through production deployment.