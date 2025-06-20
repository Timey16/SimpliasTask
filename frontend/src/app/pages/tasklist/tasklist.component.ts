import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { projectTitle } from '../../../main';
import { AuthService } from '../../services/auth.service';
import { TaskService } from '../../services/task.service';
import { TaskModel } from '../../shared/models/task-model';
import { TaskModalComponent } from '../taskModal/taskModal.component';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'tasklist',
  templateUrl: './tasklist.component.html',
  styleUrls: ['./tasklist.component.scss']
})
export class TaskListComponent implements OnInit, OnDestroy {
  private subscription = new Subscription();
  public tasks: TaskModel[] = [];
  public createModalVisible = false;

  @ViewChild('taskModal')
  public taskModal!: TaskModalComponent;

  constructor(
    private title: Title,
    private taskService: TaskService,
    private cd: ChangeDetectorRef,
    private authService: AuthService,
    private router: Router
  ) { }

  public ngOnInit(): void {
    this.subscription.add(this.taskService.getTasks().subscribe((tasks) => {
      if (!this.authService.isLoggedIn()) {
        this.router.navigate(['/']);
        return;
      }

      this.tasks = tasks;
      this.cd.markForCheck();
    }));

    this.title.setTitle(`${projectTitle} - Tasks`)
  }

  deleteTask(id: number) {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
      return;
    }

    this.taskService.deleteTask(id);
  }

  public completeTask(id: number) {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
      return;
    }

    this.taskService.completeTask(id);
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public showModal(): void {
    this.createModalVisible = true;
    this.cd.markForCheck();
  }

  public logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  public handleCancel(): void {
    this.createModalVisible = false;
    this.taskModal.resetForm();
    this.cd.markForCheck();
  }

  public handleOk(): void {
    this.createModalVisible = false;
    this.taskModal.createTask();
    this.cd.markForCheck();
  }
}
