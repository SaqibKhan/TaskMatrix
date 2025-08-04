import React, { useEffect, useState } from 'react';
import AppTaskForm from "./AppTaskForm";
import { Modal } from '@mui/material'; // Assuming MUI is available, otherwise use a custom modal

export interface IAppTask {
  id: number;
  title: string;
  description: string;
  priority: string;
  dueDate: string;
  status: string;
}

const API_URL = 'https://localhost:7127/AppTask';

const TaskList: React.FC = () => {
  const [tasks, setTasks] = useState<IAppTask[]>([]);
  const [editingTask, setEditingTask] = useState<IAppTask | null>(null);
  const [showForm, setShowForm] = useState(false);

  const fetchTasks = () => {
    fetch(API_URL)
      .then(res => res.json())
      .then(data => setTasks(data))
      .catch(() => setTasks([]));
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  const handleEdit = (task: IAppTask) => {
    setEditingTask(task);
    setShowForm(true);
  };

  const handleDelete = async (id: number) => {
    await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
    fetchTasks();
  };

  const handleAdd = () => {
    setEditingTask(null);
    setShowForm(true);
  };

  const handleFormClose = (refresh: boolean = false) => {
    setShowForm(false);
    setEditingTask(null);
    if (refresh) fetchTasks();
  };

  return (
    <div className="task-list-container">
      <button onClick={handleAdd}>Add Task</button>
      <Modal open={showForm} onClose={() => handleFormClose(false)}>
        <div style={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          borderRadius: '8px',
          minWidth: '400px'
        }} className="app-task-form-container">
          <AppTaskForm
            task={editingTask}
            onClose={handleFormClose}
          />
        </div>
      </Modal>
      <table>
        <thead>
          <tr>
            <th>ID</th><th>Title</th><th>Description</th><th>Priority</th><th>Due Date</th><th>Status</th><th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {tasks.map(task => (
            <tr key={task.id}>
              <td>{task.id}</td>
              <td>{task.title}</td>
              <td>{task.description}</td>
              <td>{task.priority}</td>
              <td>{task.dueDate}</td>
              <td>{task.status}</td>
              <td>
                <button onClick={() => handleEdit(task)}>Edit</button>
                <button onClick={() => handleDelete(task.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default TaskList;


