import { Reservation } from './Reservation';
import { Inject, Injectable } from '@nestjs/common';
import client, { Channel, Connection } from 'amqplib';

const QUEUE_NAME = process.env.RABBITMQ_QUEUE || 'test';

@Injectable()
export class AppService {
  constructor(@Inject('RabbitMQ') private channel: Channel) {
    channel.assertQueue(QUEUE_NAME, {
      durable: false,
      exclusive: false,
      autoDelete: false,
    });
  }

  getHello(): string {
    return 'Hello World!';
  }
  reserveHotel(res: Reservation): boolean {
    return this.channel.sendToQueue(QUEUE_NAME, Buffer.from(JSON.stringify(res)));
  }
}
