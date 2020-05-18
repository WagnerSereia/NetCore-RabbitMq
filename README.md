# NetCore-RabbitMq
Projeto que demonstra a utilização dos tipos de fila e exchange do RabbitMq

########################################################
Orientações de uso do projeto RabbitMq
########################################################

1)Queues do RabbitMq
Utilizar os projetos Producer, Consumer1 e Consumer2

2)Publisher e Subscriber Fanout, onde a msgm é entregue para todos os subscribers
Utilizar os projetos Pub, Sub1 e Sub2

3)Publisher e Subscriber Direct, onde a msgm é entregue para os subscribers com a mesma routingKey configurada
Utilizar os projetos PubRoutingKey, SubRoutingKey1 e SubRoutingKey2

4)Topic, onde as mensagens são encaminhadas com uma routingKey e somente os subscriber com a mesma routingKey configurada receberao
