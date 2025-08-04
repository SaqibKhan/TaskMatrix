import React, { useState, useEffect } from 'react';
import axios from 'axios';

// Define IAppTask type locally since TaskList.tsx does not export it
export interface IAppTask {
    id: number;
    title: string;
    description: string;
    priority: number;
    dueDate: string; 
    status: number;
}

interface AppTaskFormProps {
    task: IAppTask | null;
    onClose: (refresh?: boolean) => void;
}
const API_URL = 'https://localhost:7127/AppTask';

const AppTaskForm: React.FC<AppTaskFormProps> = ({ task, onClose }) => {
    const [formData, setFormData] = useState<IAppTask>({
        id: 0,
        title: '',
        description: '',
        priority: 0,
        dueDate: '',
        status: 0
    });
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    useEffect(() => {
        if (task) {
            setFormData({
                ...task,
                dueDate: task.dueDate ? formatDate(task.dueDate) : ''
            });
        } else {
            setFormData({
                id: 0,
                title: '',
                description: '',
                priority: 0,
                dueDate: '',
                status: 0
            });
        }
    }, [task]);

    // Helper to format date to yyyy-MM-dd for input[type="date"]
    function formatDate(date: string | Date): string {
        const d = new Date(date);
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

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
        try {
            if (task) {
                // Edit: use UpdateAppTaskDto
                const updateDto =  {
                    id: formData.id,
                    title: formData.title,
                    description: formData.description,
                    priority: Number(formData.priority),
                    dueDate: formData.dueDate,
                    status: Number(formData.status)
                };
                await axios.put(API_URL, updateDto);
            } else {
                // Add: use CreateAppTaskDto (no id)
                const createDto =  {
                    title: formData.title,
                    description: formData.description,
                    priority: Number(formData.priority),
                    dueDate: formData.dueDate,
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

    return (
                <div>
                    <h2>{task ? 'Edit Task' : 'Add Task'}</h2>
                    {errorMessage && <div style={{ color: 'red' }}>{errorMessage}</div>}
            <form onSubmit={handleSubmit}>

                <table>
                    <tr>
                    <td>  <label>Title:</label></td>
                     <td> <input name="title" value={formData.title} onChange={handleChange} required /></td>
                    </tr>
                    <tr>
                        <td> <label>Description:</label></td>
                        <td> <input name="description" value={formData.description} onChange={handleChange} required /></td>
                    </tr>

                    <tr>
                    <td> <label>Priority:</label></td>
                    <td><select name="priority" value={formData.priority} onChange={handleChange} required>
                        <option value="-1">Select</option>
                        <option value="1">High</option>
                        <option value="2">Normal</option>
                        <option value="3">Low</option>
                    </select></td>
                    </tr>
                    <tr>
                        <td> <label>Due Date:</label></td>
                        <td><input
                                type="date"
                                name="dueDate"
                                value={formData.dueDate}
                                onChange={handleChange}
                                required
                            /></td>
                    </tr>

                    <tr>
                        <td> <label>Status:</label></td>
                        <td> <select name="status" value={formData.status} onChange={handleChange} required>
                            <option value="-1">Select</option>
                            <option value="1">Pending</option>
                            <option value="2">In Progress</option>
                            <option value="3">Completed</option>
                            <option value="4">Deleted</option>
                        </select></td>
                    </tr>
                    <tr>
                        <td><button type="submit">{task ? 'Update' : 'Add'}</button></td>
                        <td> <button type="button" onClick={() => onClose(false)}>Cancel</button></td>
                    </tr>
                </table>

                
               
            </form>
        </div>
    );
};

export default AppTaskForm;