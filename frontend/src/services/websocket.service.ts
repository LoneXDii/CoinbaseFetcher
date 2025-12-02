import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { TickData } from '../interfaces/tickData.interface';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private hubConnection: HubConnection;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/coinbase/trades`)
      .withAutomaticReconnect()
      .build();
  }

  async connect(): Promise<void> {
    if (this.hubConnection.state === HubConnectionState.Disconnected) {
      try {
        await this.hubConnection.start();
        console.log('Connection started');
      } catch (err) {
        console.error('Error while starting connection:', err);
        throw err;
      }
    }
  }

  async disconnect(): Promise<void> {
    if (this.hubConnection.state !== HubConnectionState.Disconnected) {
      try {
        await this.hubConnection.stop();
        console.log('Connection stopped');
      } catch (err) {
        console.error('Error while stopping connection:', err);
        throw err;
      }
    }
  }

  subscribeToTrades(callback: (tickData: TickData) => void) {
    this.hubConnection.on("ReceiveTickData", (tickData: TickData) => {
      callback(tickData);
    })
  }

  unsubscribeFromTrades() {
    this.hubConnection.off("ReceiveTickData");
  }
}
