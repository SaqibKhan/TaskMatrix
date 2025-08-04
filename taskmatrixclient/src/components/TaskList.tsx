import React, { useEffect, useState, useRef } from 'react';
import AppTaskForm from "./AppTaskForm";
import { Modal } from '@mui/material';
import { useInView } from 'react-intersection-observer';

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

  const fetchTasks = async (reset = false) => {
    if (loadingRef.current || !hasMore) return;
    loadingRef.current = true;
    const res = await fetch(`${API_URL}?skip=${reset ? 0 : skip}&take=${PAGE_SIZE}`);
    const data = await res.json();
    setTasks(prev => reset ? data : [...prev, ...data]);
    setSkip(prev => reset ? PAGE_SIZE : prev + PAGE_SIZE);
    setHasMore(data.length === PAGE_SIZE);
    loadingRef.current = false;
  };

  useEffect(() => {
    fetchTasks(true);
    // eslint-disable-next-line
  }, []);

  useEffect(() => {
    if (inView && hasMore) {
      fetchTasks();
    }
    // eslint-disable-next-line
  }, [inView, hasMore]);

  const handleEdit = (task: IAppTask) => {
    setEditingTask(task);
    setShowForm(true);
  };

  const handleDelete = async (id: number) => {
    await fetch(`https://localhost:7127/AppTask/${id}`, { method: 'DELETE' });
    setSkip(0);
    setHasMore(true);
    fetchTasks(true);
  };

  const handleAdd = () => {
    setEditingTask(null);
    setShowForm(true);
  };

  const handleFormClose = (refresh: boolean = false) => {
    setShowForm(false);
    setEditingTask(null);
    if (refresh) {
      setSkip(0);
      setHasMore(true);
      fetchTasks(true);
    }
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
          minWidth: '350px'
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
        {/* Sentinel div for intersection observer */}
        <div ref={ref} style={{ height: 1 }} />
      </table>
      {!hasMore && <div>No more tasks to load.</div>}
    </div>
  );
};

export default TaskList;


