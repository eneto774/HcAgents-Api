# HC Agents - Sistema de Chatbots com IA

Uma aplicação completa para criação e gerenciamento de chatbots personalizados com integração à OpenAI, desenvolvida com .NET 9 no backend e React + TypeScript no frontend.

## 🎯 Visão Geral do Projeto

O HC Agents permite que usuários criem chatbots personalizados definindo contextos específicos e interajam com eles através de uma interface web moderna. O sistema mantém histórico completo das conversas e utiliza a API da OpenAI para gerar respostas inteligentes.

### Funcionalidades Principais

- ✅ Criação de chatbots personalizados com contexto específico
- ✅ Interface de chat em tempo real
- ✅ Histórico completo de conversas
- ✅ Autenticação via OTP por email
- ✅ API RESTful documentada com Swagger
- ✅ Arquitetura limpa com separação de responsabilidades

## 🚀 Tecnologias Utilizadas

### Backend (.NET 9)
- **ASP.NET Core** - Framework web
- **Entity Framework Core** - ORM para acesso a dados
- **MediatR** - Padrão mediator para CQRS
- **OpenAI SDK** - Integração com ChatGPT
- **JWT Bearer** - Autenticação
- **Swagger** - Documentação da API
- **xUnit + Moq** - Testes unitários

### Frontend (React + TypeScript)
- **React 18** - Biblioteca para interfaces
- **TypeScript** - Tipagem estática
- **Vite** - Build tool e dev server
- **Tailwind CSS** - Framework CSS utilitário
- **shadcn/ui** - Componentes UI modernos
- **Axios** - Cliente HTTP
- **React Router** - Roteamento

## 📋 Pré-requisitos

- **.NET 9 SDK**
- **Node.js** (versão 18 ou superior)
- **Git**
- **MySQL** - Banco de dados

## 🛠️ Instalação e Execução

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd hcagents-solution
```

### 2. Configuração do Backend

```bash
# Navegue para o diretório da API
cd Api

# Restaure as dependências
dotnet restore

# Configure as variáveis de ambiente
# Crie um arquivo appsettings.Development.json com:
{
  "OpenAI": {
    "ApiKey": "sua-chave-openai-aqui"
  },
  "Jwt": {
    "SecretKey": "sua-chave-secreta-jwt-aqui",
    "Issuer": "HCAgents",
    "Audience": "HCAgents"
  }
}

# Crie o banco de dados
execute o script DATABASE.sql no seu MySQL

# Execute a aplicação
dotnet run
```

A API estará disponível em `https://localhost:7120` e `http://localhost:5256`

### 3. Configuração do Frontend

```bash
# Navegue para o diretório do frontend
cd ../hcagents-web

# Instale as dependências
npm install

# Configure as variáveis de ambiente
# Crie um arquivo .env:
VITE_API_BASE_URL=http://localhost:5256

# Execute a aplicação
npm run dev
```

A aplicação web estará disponível em `http://localhost:5173`

## 📁 Estrutura do Projeto

```
hcagents-solution/
├── Api/                    # Camada de apresentação (Controllers)
├── Application/            # Camada de aplicação (Use Cases, Services)
├── Domain/                 # Camada de domínio (Entities, Interfaces)
├── Infrastructure/         # Camada de infraestrutura (Data, External APIs)
├── Common/                 # Configurações compartilhadas
├── Tests/                  # Testes unitários
└── hcagents-web/          # Frontend React

hcagents-web/
├── src/
│   ├── components/        # Componentes reutilizáveis
│   ├── pages/            # Páginas da aplicação
│   ├── services/         # Serviços de API
│   └── types/            # Definições de tipos TypeScript
```

## 🔧 Scripts Disponíveis

### Backend
```bash
dotnet run              # Executar em desenvolvimento
dotnet build            # Build do projeto
dotnet test             # Executar testes
```

### Frontend
```bash
npm run dev             # Servidor de desenvolvimento
npm run build           # Build para produção
npm run lint            # Verificar código
npm run preview         # Preview da build
```

## � Documentação da API

Com o backend executando, acesse:
- **Swagger UI**: `https://localhost:7120/swagger`

### Principais Endpoints

- `POST /session/validate` - Autenticação via OTP
- `GET /chat` - Listar chatbots do usuário
- `POST /chat` - Criar novo chatbot
- `GET /message` - Buscar mensagens de um chat
- `POST /message` - Enviar mensagem para o chatbot

## 🧪 Executando Testes

```bash
cd Tests
dotnet test --verbosity normal
```

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📞 Suporte

Para dúvidas e suporte:
- Abra uma issue no GitHub
- Email: eneto774@gmail.com

---

Desenvolvido com ❤️ para o desafio técnico HighCapital

