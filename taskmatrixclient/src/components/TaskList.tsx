import React, { useEffect, useState, useRef } from 'react';
import AppTaskForm from "./AppTaskForm";
import { Modal } from '@mui/material';
import { useInView } from 'react-intersection-observer';
import TaskStatusChart from './TaskStatusChart';

export interface IAppTask {
  id: number;
  title: string;
  description: string;
  priority: string;
  dueDate: string;
  status: string;
}

const API_URL = 'https://localhost:7127/AppTask/paged';
const PAGE_SIZE = 20;

const TaskList: React.FC = () => {
  const [tasks, setTasks] = useState<IAppTask[]>([]);
  const [editingTask, setEditingTask] = useState<IAppTask | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [skip, setSkip] = useState(0);
  const [hasMore, setHasMore] = useState(true);
  const loadingRef = useRef(false);

  const { ref, inView } = useInView({
    threshold: 0,
    triggerOnce: false,
  });

  // Fetch tasks from API, optionally resetting the list, with retry logic
  const fetchTasks = async (reset = false) => {
    if (loadingRef.current || !hasMore) return;
    loadingRef.current = true;

    const currentSkip = reset ? 0 : skip;
    const maxRetries = 3;
    const retryDelay = 1000; // ms

    let attempt = 0;
    let success = false;
    let data: IAppTask[] = [];

    while (attempt < maxRetries && !success) {
      try {
        const res = await fetch(`${API_URL}?skip=${currentSkip}&take=${PAGE_SIZE}`);
        if (!res.ok) throw new Error(`HTTP error: ${res.status}`);
        data = await res.json();
        success = true;
      } catch (error) {
        attempt++;
        if (attempt < maxRetries) {
          await new Promise(resolve => setTimeout(resolve, retryDelay));
        } else {
          console.error('Failed to fetch tasks after retries:', error);
          setHasMore(false);
        }
      }
    }

    if (success) {
      setTasks(prevTasks => reset ? data : [...prevTasks, ...data]);
      setSkip(reset ? PAGE_SIZE : skip + PAGE_SIZE);
      setHasMore(data.length === PAGE_SIZE);
    }

    loadingRef.current = false;
  };

  // Initial fetch on mount
  useEffect(() => {
    fetchTasks(true);
    // eslint-disable-next-line
  }, []);

  // Fetch more tasks when the bottom of the list is in view
  useEffect(() => {
    if (inView && hasMore) {
      fetchTasks();
    }
    // eslint-disable-next-line
  }, [inView, hasMore]);

  // Open the form to edit a task
  const handleEdit = (task: IAppTask) => {
    setEditingTask(task);
    setShowForm(true);
  };

  // Delete a task and refresh the list
  const handleDelete = async (id: number) => {
    await fetch(`https://localhost:7127/AppTask/${id}`, { method: 'DELETE' });
    setSkip(0);
    setHasMore(true);
    fetchTasks(true);
  };

  // Open the form to add a new task
  const handleAdd = () => {
    setEditingTask(null);
    setShowForm(true);
  };

  // Close the form, optionally refreshing the list
  const handleFormClose = (refresh: boolean = false) => {
    setShowForm(false);
    setEditingTask(null);
    if (refresh) {
      setSkip(0);
      setHasMore(true);
      fetchTasks(true);
    }
  };

  // Add these helper functions to map numeric values to string labels
  const priorityOptions = [
    { value: -1, label: 'Select' },
    { value: 1, label: 'High' },
    { value: 2, label: 'Normal' },
    { value: 3, label: 'Low' }
  ];

  const statusOptions = [
    { value: -1, label: 'Select' },
    { value: 1, label: 'Pending' },
    { value: 2, label: 'In Progress' },
    { value: 3, label: 'Completed' },
    { value: 4, label: 'Archived' },
    { value: 5, label: 'Deleted' }
  ];

  function getPriorityLabel(value: number | string) {
    const opt = priorityOptions.find(o => o.value === Number(value));
    return opt ? opt.label : value;
  }

  function getStatusLabel(value: number | string) {
    const opt = statusOptions.find(o => o.value === Number(value));
    return opt ? opt.label : value;
  }

  return (
    <div className="task-list-container">
      <TaskStatusChart />
      <button onClick={handleAdd}>Add Task</button>
      <Modal open={showForm} onClose={() => handleFormClose(false)}>
        <div
          style={{
            position: 'absolute',
            top: '50%',
            left: '50%',
            transform: 'translate(-50%, -50%)',
            borderRadius: '8px',
            minWidth: '350px'
          }}
          className="app-task-form-container"
        >
          <AppTaskForm
            task={editingTask}
            onClose={handleFormClose}
          />
        </div>
      </Modal>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Title</th>
            <th>Description</th>
            <th>Priority</th>
            <th>Due Date</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {tasks.map(task => (
            <tr key={task.id}>
              <td>{task.id}</td>
              <td>{task.title}</td>
              <td>{task.description}</td>
              <td>{getPriorityLabel(task.priority)}</td>
              <td>{task.dueDate}</td>
              <td>{getStatusLabel(task.status)}</td>
              <td>
                <button onClick={() => handleEdit(task)}>Edit</button>
                <button onClick={() => handleDelete(task.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div ref={ref} style={{ height: 1 }} />
      {!hasMore && <div>No more tasks to load.</div>}
    </div>
  );
};

export default TaskList;


