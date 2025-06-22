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
  public createModalVisible = false;
  public deleteModalVisible = false;

  private subscription = new Subscription();
  private selectedId = 0;
  public tasks!: TaskModel[];

  @ViewChild('taskModal')
  public taskModal!: TaskModalComponent;

  constructor(
    private title: Title,
    private taskService: TaskService,
    private cd: ChangeDetectorRef,
    private authService: AuthService,
    private router: Router,
  ) { }

  public ngOnInit(): void {
    this.subscription.add(this.taskService.taskBehaviorSubject.subscribe((tasks) => {
      if (!this.authService.isLoggedIn()) {
        this.router.navigate(['/']);
        return;
      }
      if (tasks != null) {
        const taskList: TaskModel[] = [];
        for (const id in tasks) {
          taskList.push(tasks[id]);
        }

        this.tasks = taskList;
      }
      this.cd.markForCheck();
    }));

    this.title.setTitle(`${projectTitle} - Tasks`)
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public completeTask(id: number) {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
      return;
    }

    this.subscription.add(this.taskService.completeTask(id).subscribe());
    this.cd.markForCheck();
  }

  public showCreateModal(): void {
    this.createModalVisible = true;
    this.cd.markForCheck();
  }

  public showDeleteModal(id: number): void {
    this.deleteModalVisible = true;
    this.selectedId = id;
    this.cd.markForCheck();
  }

  public logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  public handleCreateCancel(): void {
    this.createModalVisible = false;
    this.taskModal.resetForm();
    this.cd.markForCheck();
  }

  public handleCreateOk(): void {
    this.createModalVisible = false;
    this.taskModal.createTask();
    this.cd.markForCheck();
  }

  public handleDeleteCancel(): void {
    this.deleteModalVisible = false;
    this.cd.markForCheck();
  }

  public handleDeleteOk(): void {
    this.deleteModalVisible = false;
    this.deleteTask(this.selectedId);
    this.selectedId = 0;
    this.cd.markForCheck();
  }


  private deleteTask(id: number) {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
      return;
    }

    this.subscription.add(this.taskService.deleteTask(id).subscribe());
  }
}
