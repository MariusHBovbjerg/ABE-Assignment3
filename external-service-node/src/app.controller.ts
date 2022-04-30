import { Reservation } from './Reservation';
import { Body, Controller, Get, Post } from '@nestjs/common';
import { AppService } from './app.service';

@Controller()
export class AppController {
  constructor(private readonly appService: AppService) {}

  @Get()
  getHello(): string {
    return this.appService.getHello();
  }
  @Post('CreateHotel')
  createHotel(@Body() res: Reservation): string {
    return String(this.appService.reserveHotel(res));
  }
}
