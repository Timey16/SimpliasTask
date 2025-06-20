export interface TaskModel {
  taskId: number,
  name: string,
  description: string,
  completed: boolean,
  creationDate: Date,
  priority: TaskPriority
}

export enum TaskPriority {
  UNSET,
  UNKNOWN,
  LOW,
  MEDIUM,
  HIGH
}
