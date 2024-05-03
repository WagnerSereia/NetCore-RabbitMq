# NetCore-RabbitMq
Projeto que demonstra a utilização dos tipos de fila e exchange do RabbitMq

------------------------------------------
Orientações de uso do projeto RabbitMq
------------------------------------------
Para rodas os projetos é necessário uma instancia do RabbitMQ rodando, para esse projeto utilizei uma imagem do docker e caso queira utilizar a imagem, no diretorio raiz do projeto existe o docker-compose utilizado, para rodar executar o comando a seguir:

No windows terminal
diretorioProjeto>docker-compose -f docker-compose.yml up

Esse comando baixara (caso não tenha) a imagem do Rabbit e subira um container com os parametros configurados do dockerFile.

Para rodar os projetos a seguir, clicar com o botão direito em cima da solução --> "set StartUp Projects.." e indicar os projetos mencionados em cada uma das opções

1)Queues do RabbitMq
Utilizar os projetos Producer, Consumer1 e Consumer2

2)Publisher e Subscriber Fanout, onde a msgm é entregue para todos os subscribers
Utilizar os projetos Pub, Sub1 e Sub2

3)Publisher e Subscriber Direct, onde a msgm é entregue para os subscribers com a mesma routingKey configurada
Utilizar os projetos PubRoutingKey, SubRoutingKey1 e SubRoutingKey2

4)Topic, onde as mensagens são encaminhadas com uma routingKey e somente os subscriber com a mesma routingKey configurada receberao
## Parceiro
Esse é um desenvolvimento de pesquisa e parceria com [ConnectionPharma](https://cpharma.com.br)
