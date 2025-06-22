import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, map, Observable } from "rxjs";
import { environment } from "../../environments/environment";
import { TaskResponseModel } from "../shared/models/rask-response-model";
import { TaskModel, TaskPriority } from "../shared/models/task-model";
import { AuthService } from "./auth.service";

export interface TaskList {
  [taskId: number]: TaskModel
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  public taskBehaviorSubject = new BehaviorSubject<TaskList | null>(null);

  private apiUrl = environment.apiUrl;
  private tasks: TaskList = { };
  private get headers() {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.authService.retrieveToken()}`
    })
  }

  constructor(private http: HttpClient, private authService: AuthService) {
    this.getInitialTasks().subscribe();
  }

  public createTask(taskModel: TaskModel): Observable<TaskResponseModel> {
    return this.http.post<TaskResponseModel>(`${this.apiUrl}tasks`, taskModel, { headers: this.headers })
      .pipe(
        map((res) => {
          return res;
        })
      )
  }

  public createTaskFromMessage(taskModel: TaskModel): void {
    this.tasks[taskModel.taskId] = taskModel;
    this.taskBehaviorSubject.next(this.tasks);
  }

  public completeTask(id: number): Observable<TaskResponseModel> {
    return this.http.put<TaskResponseModel>(`${this.apiUrl}tasks/${id}/complete`, null, { headers: this.headers })
      .pipe(
        map((res) => {
          return res;
        })
      )
  }

  public completeTaskFromMessage(id: number): void {
    console.log('completed',id)
    this.tasks[id].completed = true;
    this.taskBehaviorSubject.next(this.tasks);
  }

  public deleteTask(id: number): Observable<TaskResponseModel[]> {
    return this.http.delete<TaskResponseModel[]>(`${this.apiUrl}tasks/${id}`, { headers: this.headers })
      .pipe(
        map((res) => {
          return res;
        })
      )
  }

  public deleteTaskFromMessage(id: number): void {
    delete this.tasks[id];
    this.taskBehaviorSubject.next(this.tasks);
  }

  public updatePriorityFromMessage(id: number, priority: TaskPriority): void {
    this.tasks[id].priority = priority;
    this.taskBehaviorSubject.next(this.tasks);
  }

  private getInitialTasks(): Observable<TaskModel[]> {
    return this.http.get<TaskModel[]>(`${this.apiUrl}tasks`, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.authService.retrieveToken()}`
      })
    })
      .pipe(
        map((res) => {
          for (const task of res) {
            this.tasks[task.taskId] = task;
          }
          this.taskBehaviorSubject.next(this.tasks);
          return res;
        })
      )
  }
}
