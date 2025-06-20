import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment.prod';
import { TaskModel, TaskPriority } from '../shared/models/task-model';
import { TaskService } from './task.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  constructor(private taskService: TaskService) { }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRUrl) // URL of the SignalR hub
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connection started'))
      .catch(err => console.log('Error establishing SignalR connection: ' + err));
  }

  public addMessageListener = () => {
    this.hubConnection.on('ReceiveMessage', (user: string, message: string) => {
      console.log(`User: ${user}, Message: ${message}`);
    });

    this.hubConnection.on('TaskCreated', (task: TaskModel) => {
      this.taskService.createTaskFromMessage(task);
    });

    this.hubConnection.on('TaskDeleted', (id: number) => {
      this.taskService.deleteTaskFromMessage(id);
    });

    this.hubConnection.on('TaskCompleted', (id: number) => {
      this.taskService.completeTaskFromMessage(id);
    });

    this.hubConnection.on('TaskPriority', (id: number, priority: TaskPriority) => {
      this.taskService.updatePriorityFromMessage(id, priority);
    });
  }

  public sendMessage = (user: string, message: string) => {
    this.hubConnection.invoke('SendMessage', user, message)
      .catch(err => console.error(err));
  }
}
