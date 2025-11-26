import { Component, OnDestroy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { WebSocketService } from '../services/websocket.service';
import { TickData } from '../interfaces/tickData.interface';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnDestroy {
  trades: TickData[] = [];
  isSubscribed: boolean = false;

  constructor(private webSocketService: WebSocketService) {}

  ngOnDestroy(): void {
    if (this.isSubscribed) {
      this.unsubscribe().catch(err => console.error('Error during unsubscribe in ngOnDestroy:', err));
    }
  }

  toggleSubscription(): void {
    if (this.isSubscribed) {
      this.unsubscribe().catch(err => console.error('Error during unsubscribe:', err));
    } else {
      this.subscribe().catch(err => console.error('Error during subscribe:', err));
    }
  }

  private async subscribe(): Promise<void> {
    try {
      await this.webSocketService.connect();
      this.webSocketService.subscribeToTrades((tickData: TickData) => {
        this.trades.unshift(tickData);
      });
      this.isSubscribed = true;
    } catch (err) {
      console.error('Failed to subscribe:', err);
      this.isSubscribed = false;
      throw err;
    }
  }

  private async unsubscribe(): Promise<void> {
    try {
      this.webSocketService.unsubscribeFromTrades();
      await this.webSocketService.disconnect();
      this.isSubscribed = false;
    } catch (err) {
      console.error('Failed to unsubscribe:', err);
      this.isSubscribed = false;
      throw err;
    }
  }
}
