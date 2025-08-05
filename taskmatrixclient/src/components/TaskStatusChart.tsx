import React, { useEffect, useState } from 'react';
import { PieChart } from '@mui/x-charts/PieChart';
        
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
  priority: string;
  dueDate: string;
  status: TaskStatus;
}

const API_URL = 'https://localhost:7127/AppTask';
const token = localStorage.getItem('jwtToken');

const statusLabels: Record<TaskStatus, string> = {
  [TaskStatus.Pending]: 'Pending',
  [TaskStatus.InProgress]: 'In Progress',
  [TaskStatus.Completed]: 'Completed',
  [TaskStatus.Archived]: 'Archived',
  [TaskStatus.Deleted]: 'Deleted'
};

const TaskStatusChart: React.FC = () => {
  const [pieData, setPieData] = useState<{ id: number, value: number, label: string }[]>([]);

  useEffect(() => {
    const fetchTasks = async () => {
        const res = await fetch(API_URL, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });;
      const data: IAppTask[] = await res.json();
      const statusCount: Record<TaskStatus, number> = {
        [TaskStatus.Pending]: 0,
        [TaskStatus.InProgress]: 0,
        [TaskStatus.Completed]: 0,
        [TaskStatus.Archived]: 0,
        [TaskStatus.Deleted]: 0
      };
      data.forEach(task => {
        statusCount[task.status] = (statusCount[task.status] || 0) + 1;
      });
      const total = data.length;
      const chartData = Object.entries(statusCount)
        .filter(([_, value]) => value > 0)
        .map(([key, value], idx) => ({
          id: idx,
          value: Math.round((value / total) * 100),
          label: `${statusLabels[Number(key) as TaskStatus]} (${value})`
        }));
      setPieData(chartData);
    };
    fetchTasks();
  }, []);

  return (
    <div style={{ maxWidth: 400, margin: '24px auto' }}>
      <h3>Task Status Distribution</h3>
      <PieChart
        series={[
          {
            data: pieData,
            innerRadius: 60,
            outerRadius: 100,
            paddingAngle: 2,
            cornerRadius: 5,
          },
        ]}
        width={400}
        height={250}
      />
    </div>
  );
};

export default TaskStatusChart;