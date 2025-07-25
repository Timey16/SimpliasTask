import { Component } from '@angular/core';
import { SignalRService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  constructor(private signalRService: SignalRService) {
    this.signalRService.startConnection();
    this.signalRService.addMessageListener();
  }
}
