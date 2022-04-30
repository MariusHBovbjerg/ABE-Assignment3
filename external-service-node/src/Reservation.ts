import { ApiProperty } from '@nestjs/swagger';

export class Reservation {
  @ApiProperty()
  hotelId: number;
  @ApiProperty()
  checkIn: Date;
  @ApiProperty()
  checkOut: Date;
  @ApiProperty()
  roomNo: number;
  @ApiProperty()
  customerName: string;
  @ApiProperty()
  customerEmail: string;
  @ApiProperty()
  customerAddress: string;
}
