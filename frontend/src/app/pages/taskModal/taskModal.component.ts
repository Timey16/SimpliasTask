import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { projectTitle } from '../../../main';
import { AuthService } from '../../services/auth.service';
import { TaskService } from '../../services/task.service';
import { TaskModel, TaskPriority } from '../../shared/models/task-model';

export interface Empty {

}

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'task-modal',
  templateUrl: './taskModal.component.html',
  styleUrls: ['./taskModal.component.scss']
})
export class TaskModalComponent implements OnInit, OnDestroy {
  public taskForm!: FormGroup;
  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private title: Title,
    private taskService: TaskService,
    private authService: AuthService,
    private router: Router,
  ) { }

  createTask() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
    }

    for (const i in this.taskForm.controls) {
      this.taskForm.controls[i].markAsDirty();
      this.taskForm.controls[i].updateValueAndValidity();
    }

    const taskModel: TaskModel = {
      id: 0,
      name: this.taskForm.controls['name'].value,
      description: this.taskForm.controls['description'].value,
      completed: false,
      creationDate: new Date(),
      priority: TaskPriority.UNSET,
    }

    this.subscription.add(this.taskService.createTask(taskModel).subscribe({
      next: (response) => {
        if (response.result) {
          console.log('task creation succeeded');
        }
      },
      error: (error) => {
        console.error('registering failed');
        console.error(error);
      }
    }));

    this.taskForm.reset();
  }

  resetForm() {
    this.taskForm.reset();
  }

  ngOnInit(): void {
    this.title.setTitle(`${projectTitle} - Register`)
    this.taskForm = this.fb.group({
      name: [null, [Validators.required]],
      description: [null, [Validators.required, Validators.maxLength(280)]],
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
