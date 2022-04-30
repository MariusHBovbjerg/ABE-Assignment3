import { Channel } from 'amqp-connection-manager';
import client, { connect, Connection } from 'amqplib';
import { Logger, Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { create } from 'domain';

const createChannel = async (): Promise<Channel> => {
  const connString =
    'amqp://' +
    (process.env.RABBITMQ_USER || 'guest') +
    ':' +
    (process.env.RABBITMQ_PASS || 'guest') +
    '@' +
    (process.env.RABBITMQ_HOST || 'localhost') +
    ':' +
    (process.env.RABBITMQ_PORT || '5672');
  new Logger('AMQP').log(`Connecting to RabbitMQ at ${connString}`);

  try {
    const conn: Connection = await connect(connString);
    return conn.createChannel();
  } catch (E) {
    new Logger('AMQP').error(`Connection refused: ${E.message}`);
    await new Promise(resolve => setTimeout(resolve, 1000));
    return createChannel();
  }
};
const connFactory = async (): Promise<Channel> => {
  const c = await createChannel();
  new Logger('AMQP').log('Connected to RabbitMQ');
  return c;
};

@Module({
  imports: [],
  controllers: [AppController],
  providers: [
    AppService,
    {
      provide: 'RabbitMQ',
      useFactory: connFactory,
    },
  ],
})
export class AppModule {}
