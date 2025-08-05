import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Modal } from '@mui/material';
import { priorityOptions, statusOptions, type AppTaskFormProps, type IAppTask } from './TaskTypes';
// Define IAppTask type locally since TaskList.tsx does not export it


const defaultTask: IAppTask = {
    id: 0,
    title: '',
    description: '',
    priority: 0,
    dueDate: '',
    status: 0
};

// Helper to format date to yyyy-MM-dd for input[type="date"]
const API_URL = 'https://localhost:7127/AppTask';
const token = localStorage.getItem('jwtToken');
axios.defaults.headers.common = { 'Authorization': `Bearer ${token}` }
function formatDate(date: string | Date): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

const AppTaskForm: React.FC<AppTaskFormProps> = ({ task, onClose }) => {
    const [formData, setFormData] = useState<IAppTask>(defaultTask);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [showConfirm, setShowConfirm] = useState(false);
    const [pendingSubmit, setPendingSubmit] = useState(false);

    useEffect(() => {
        if (task) {
            setFormData({
                ...task,
                dueDate: task.dueDate ? formatDate(task.dueDate) : ''
            });
        } else {
            setFormData(defaultTask);
        }
    }, [task]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setErrorMessage(null);

        if (Number(formData.priority) === 1 && !showConfirm) {
            setShowConfirm(true);
            setPendingSubmit(true);
            return;
        }

        try {
            if (task) {
                // Edit: use UpdateAppTaskDto
                const updateDto = {
                    ...formData,
                    priority: Number(formData.priority),
                    status: Number(formData.status)
                };
                await axios.put(API_URL, updateDto);
            } else {
                // Add: use CreateAppTaskDto (no id)
                const { id, ...createDto } = {
                    ...formData,
                    priority: Number(formData.priority),
                    status: Number(formData.status)
                };
                await axios.post(API_URL, createDto);
            }
            onClose(true);
        } catch (error) {
            if (axios.isAxiosError(error)) {
                const serverMessage =
                    error.response?.data?.message ||
                    error.response?.data?.title ||
                    error.response?.data?.error ||
                    'Failed to submit task. Please check your input.';
                setErrorMessage(serverMessage);
            } else {
                setErrorMessage('Failed to submit task. Please check your input.');
            }
            console.error(error);
        }
    };

    const handleConfirm = async () => {
        setShowConfirm(false);
        setPendingSubmit(false);
        // Manually trigger submit after confirmation
        try {
            if (task) {
                const updateDto = {
                    ...formData,
                    priority: Number(formData.priority),
                    status: Number(formData.status)
                };
                await axios.put(API_URL, updateDto);
            } else {
                const { id, ...createDto } = {
                    ...formData,
                    priority: Number(formData.priority),
                    status: Number(formData.status)
                };
                await axios.post(API_URL, createDto);
            }
            onClose(true);
        } catch (error) {
            if (axios.isAxiosError(error)) {
                const serverMessage =
                    error.response?.data?.message ||
                    error.response?.data?.title ||
                    error.response?.data?.error ||
                    'Failed to submit task. Please check your input.';
                setErrorMessage(serverMessage);
            } else {
                setErrorMessage('Failed to submit task. Please check your input.');
            }
            console.error(error);
        }
    };

    const handleCancelConfirm = () => {
        setShowConfirm(false);
        setPendingSubmit(false);
    };

    return (
        <div>
            <h2>{task ? 'Edit Task' : 'Add Task'}</h2>
            {errorMessage && <div style={{ color: 'red' }}>{errorMessage}</div>}
            <form onSubmit={handleSubmit}>
                <table>
                    <tbody>
                        <tr>
                            <td><label htmlFor="title">Title:</label></td>
                            <td>
                                <input
                                    id="title"
                                    name="title"
                                    value={formData.title}
                                    onChange={handleChange}
                                    required
                                />
                            </td>
                        </tr>
                        <tr>
                            <td><label htmlFor="description">Description:</label></td>
                            <td>
                                <input
                                    id="description"
                                    name="description"
                                    value={formData.description}
                                    onChange={handleChange}
                                    required
                                />
                            </td>
                        </tr>
                        <tr>
                            <td><label htmlFor="priority">Priority:</label></td>
                            <td>
                                <select
                                    id="priority"
                                    name="priority"
                                    value={formData.priority}
                                    onChange={handleChange}
                                    required
                                >
                                    {priorityOptions.map(opt => (
                                        <option key={opt.value} value={opt.value}>{opt.label}</option>
                                    ))}
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td><label htmlFor="dueDate">Due Date:</label></td>
                            <td>
                                <input
                                    id="dueDate"
                                    type="date"
                                    name="dueDate"
                                    value={formData.dueDate}
                                    onChange={handleChange}
                                    required
                                />
                            </td>
                        </tr>
                        <tr>
                            <td><label htmlFor="status">Status:</label></td>
                            <td>
                                <select
                                    id="status"
                                    name="status"
                                    value={formData.status}
                                    onChange={handleChange}
                                    required
                                >
                                    {statusOptions.map(opt => (
                                        <option key={opt.value} value={opt.value}>{opt.label}</option>
                                    ))}
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <button type="submit">{task ? 'Update' : 'Add'}</button>
                            </td>
                            <td>
                                <button type="button" onClick={() => onClose(false)}>Cancel</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </form>
            <Modal open={showConfirm} onClose={handleCancelConfirm}>
                <div
                    style={{
                        position: 'absolute',
                        top: '50%',
                        left: '50%',
                        transform: 'translate(-50%, -50%)',
                        background: '#fff',
                        padding: '24px',
                        borderRadius: '8px',
                        minWidth: '300px',
                        boxShadow: '0 2px 8px rgba(0,0,0,0.2)'
                    }}
                >
                    <h3>Confirm High Priority</h3>
                    <p>Are you sure you want to set this task to <b>High</b> priority?</p>
                    <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px' }}>
                        <button onClick={handleConfirm}>Yes, Set High Priority</button>
                        <button onClick={handleCancelConfirm}>Cancel</button>
                    </div>
                </div>
            </Modal>
        </div>
    );
};

export default AppTaskForm;