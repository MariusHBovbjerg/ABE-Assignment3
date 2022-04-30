import client, { connect, Connection } from 'amqplib';
import { Logger, Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';

@Module({
  imports: [],
  controllers: [AppController],
  providers: [
    AppService,
    {
      provide: 'RabbitMQ',
      useFactory: async () => {
        const connString =
          'amqp://' +
          (process.env.RABBITMQ_USER || 'guest') +
          ':' +
          (process.env.RABBITMQ_PASS || 'guest') +
          '@' +
          (process.env.RABBITMQ_HOST || 'localhost') +
          ':' +
          (process.env.RABBITMQ_PORT || '5672');
        Logger.log(`Connecting to RabbitMQ at ${connString}`);
        const conn: Connection = await connect(connString);
        return conn.createChannel();
      },
    },
  ],
})
export class AppModule {}
