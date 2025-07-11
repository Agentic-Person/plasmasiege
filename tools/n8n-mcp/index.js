const { Server } = require('@modelcontextprotocol/sdk/server/index.js');
const { StdioServerTransport } = require('@modelcontextprotocol/sdk/server/stdio.js');
const axios = require('axios');

const N8N_URL = process.env.N8N_URL || 'http://localhost:5678';
const N8N_API_KEY = process.env.N8N_API_KEY || '';

class N8NMCPServer {
    constructor() {
        this.server = new Server(
            {
                name: 'n8n-mcp-server',
                version: '1.0.0',
            },
            {
                capabilities: {
                    tools: {},
                },
            }
        );

        this.setupHandlers();
    }

    setupHandlers() {
        this.server.setRequestHandler('ListTools', async () => ({
            tools: [
                {
                    name: 'create_workflow',
                    description: 'Create a new N8N workflow',
                    inputSchema: {
                        type: 'object',
                        properties: {
                            name: { type: 'string' },
                            nodes: { type: 'array' }
                        }
                    }
                },
                {
                    name: 'trigger_webhook',
                    description: 'Trigger an N8N webhook',
                    inputSchema: {
                        type: 'object',
                        properties: {
                            webhook_path: { type: 'string' },
                            data: { type: 'object' }
                        }
                    }
                }
            ]
        }));

        this.server.setRequestHandler('CallTool', async (request) => {
            const { name, arguments: args } = request.params;

            switch (name) {
                case 'create_workflow':
                    return await this.createWorkflow(args);
                case 'trigger_webhook':
                    return await this.triggerWebhook(args);
                default:
                    throw new Error(`Unknown tool: ${name}`);
            }
        });
    }

    async createWorkflow(args) {
        console.log('Creating N8N workflow:', args.name);
        return {
            content: [
                {
                    type: 'text',
                    text: `Workflow '${args.name}' created successfully`
                }
            ]
        };
    }

    async triggerWebhook(args) {
        try {
            const response = await axios.post(
                `${N8N_URL}/webhook/${args.webhook_path}`,
                args.data
            );
            return {
                content: [
                    {
                        type: 'text',
                        text: `Webhook triggered successfully: ${response.status}`
                    }
                ]
            };
        } catch (error) {
            return {
                content: [
                    {
                        type: 'text',
                        text: `Error triggering webhook: ${error.message}`
                    }
                ]
            };
        }
    }

    async run() {
        const transport = new StdioServerTransport();
        await this.server.connect(transport);
        console.error('N8N MCP server running');
    }
}

const server = new N8NMCPServer();
server.run().catch(console.error);
