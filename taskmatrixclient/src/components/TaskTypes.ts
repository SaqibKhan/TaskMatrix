export enum TaskStatus {
  Pending = 1,
  InProgress = 2,
  Completed = 3,
  Archived = 4,
  Deleted = 5
}

export interface IAppTask {
  id: number;
  title: string;
  description: string;
  priority: number | string;
  dueDate: string;
  status: number | string;
}

export interface IOption {
  value: number;
  label: string;
}

export const priorityOptions: IOption[] = [
  { value: -1, label: 'Select' },
  { value: 1, label: 'High' },
  { value: 2, label: 'Normal' },
  { value: 3, label: 'Low' }
];

export const statusOptions: IOption[] = [
  { value: -1, label: 'Select' },
  { value: 1, label: 'Pending' },
  { value: 2, label: 'In Progress' },
  { value: 3, label: 'Completed' },
  { value: 4, label: 'Archived' },
  { value: 5, label: 'Deleted' }
];

export const statusLabels: Record<TaskStatus, string> = {
  [TaskStatus.Pending]: 'Pending',
  [TaskStatus.InProgress]: 'In Progress',
  [TaskStatus.Completed]: 'Completed',
  [TaskStatus.Archived]: 'Archived',
  [TaskStatus.Deleted]: 'Deleted'
};
export interface AppTaskFormProps {
    task: IAppTask | null;
    onClose: (refresh?: boolean) => void;
}