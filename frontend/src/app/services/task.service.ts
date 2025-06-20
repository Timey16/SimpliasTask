import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, map, Observable, of } from "rxjs";
import { environment } from "../../environments/environment";
import { TaskResponseModel } from "../shared/models/rask-response-model";
import { TaskModel, TaskPriority } from "../shared/models/task-model";
import { AuthService } from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = environment.apiUrl;
  private tasks: TaskModel[] = [];
  private taskBehaviorSubject = new BehaviorSubject<TaskModel[]>([]);
  private get headers() {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.authService.retrieveToken()}`
    })
  }

  constructor(private http: HttpClient, private authService: AuthService) { }

  public createTask(taskModel: TaskModel): Observable<TaskResponseModel> {
    return this.http.post<TaskResponseModel>(`${this.apiUrl}tasks`, taskModel, { headers: this.headers })
      .pipe(
        map((res) => {
          return res;
        })
      )
  }

  public createTaskFromMessage(taskModel: TaskModel): void {
    this.tasks.push(taskModel);
    this.refreshTasks();
  }

  public getTasks(fullRefresh = false): Observable<TaskModel[]> {
    if (this.tasks.length > 0 && !fullRefresh) {
      return this.refreshTasks();
    }

    return this.http.get<TaskModel[]>(`${this.apiUrl}tasks`, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${this.authService.retrieveToken()}`
      }) })
      .pipe(
        map((res) => {
          for (const task of res) {
            this.tasks[task.taskId] = task;
          }
          this.tasks = res;
          this.taskBehaviorSubject.next(this.tasks);
          return res;
        })
      )
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
    this.tasks[id].completed = true;
    this.taskBehaviorSubject.next(this.tasks);
    this.refreshTasks();
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
    this.tasks.splice(id);
    this.refreshTasks();
  }

  public updatePriorityFromMessage(id: number, priority: TaskPriority): void {
    this.tasks[id].priority = priority;
    this.taskBehaviorSubject.next(this.tasks);
    this.refreshTasks();
  }

  private refreshTasks(): Observable<TaskModel[]> {
    this.taskBehaviorSubject.next(this.tasks);
    return this.taskBehaviorSubject.asObservable();
  }
}
