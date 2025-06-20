export interface TaskModel {
  id: number,
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
