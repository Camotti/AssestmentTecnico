import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { lessonService, courseService } from '../services/api';
import './Lessons.css';

function Lessons() {
    const { courseId } = useParams();
    const navigate = useNavigate();
    const [lessons, setLessons] = useState([]);
    const [courseName, setCourseName] = useState('');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingLesson, setEditingLesson] = useState(null);
    const [lessonTitle, setLessonTitle] = useState('');
    const [lessonOrder, setLessonOrder] = useState('');

    useEffect(() => {
        loadLessons();
        loadCourse();
    }, [courseId]);

    const loadCourse = async () => {
        try {
            const data = await courseService.getCourseSummary(courseId);
            setCourseName(data.title);
        } catch (err) {
            console.error(err);
        }
    };

    const loadLessons = async () => {
        try {
            setLoading(true);
            const data = await lessonService.getLessonsByCourse(courseId);
            setLessons(data);
            setError('');
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            await lessonService.createLesson(courseId, lessonTitle, parseInt(lessonOrder));
            setShowModal(false);
            setLessonTitle('');
            setLessonOrder('');
            loadLessons();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleUpdate = async (e) => {
        e.preventDefault();
        try {
            await lessonService.updateLesson(editingLesson.id, lessonTitle, parseInt(lessonOrder));
            setShowModal(false);
            setEditingLesson(null);
            setLessonTitle('');
            setLessonOrder('');
            loadLessons();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleDelete = async (id) => {
        if (!confirm('Are you sure you want to delete this lesson?')) return;
        try {
            await lessonService.deleteLesson(id);
            loadLessons();
        } catch (err) {
            setError(err.message);
        }
    };

    const handleReorder = async (id, direction) => {
        try {
            await lessonService.reorderLesson(id, direction);
            loadLessons();
        } catch (err) {
            alert(err.message);
        }
    };

    const openCreateModal = () => {
        setEditingLesson(null);
        setLessonTitle('');
        const nextOrder = lessons.length > 0 ? Math.max(...lessons.map(l => l.order)) + 1 : 1;
        setLessonOrder(nextOrder.toString());
        setShowModal(true);
    };

    const openEditModal = (lesson) => {
        setEditingLesson(lesson);
        setLessonTitle(lesson.title);
        setLessonOrder(lesson.order.toString());
        setShowModal(true);
    };

    return (
        <div className="lessons-container">
            <header className="lessons-header">
                <button onClick={() => navigate('/courses')} className="btn-back">← Back to Courses</button>
                <h1>📖 {courseName || 'Course Lessons'}</h1>
                <button onClick={openCreateModal} className="btn-primary">+ New Lesson</button>
            </header>

            {error && <div className="error-message">{error}</div>}

            {loading ? (
                <div className="loading">Loading...</div>
            ) : lessons.length === 0 ? (
                <div className="empty-state">
                    <p>No lessons yet. Create your first lesson!</p>
                </div>
            ) : (
                <div className="lessons-list">
                    {lessons.map((lesson, index) => (
                        <div key={lesson.id} className="lesson-card">
                            <div className="lesson-order">#{lesson.order}</div>
                            <div className="lesson-content">
                                <h3>{lesson.title}</h3>
                            </div>
                            <div className="lesson-actions">
                                <div className="reorder-buttons">
                                    <button
                                        onClick={() => handleReorder(lesson.id, 'up')}
                                        disabled={index === 0}
                                        className="btn-icon"
                                        title="Move up"
                                    >
                                        ↑
                                    </button>
                                    <button
                                        onClick={() => handleReorder(lesson.id, 'down')}
                                        disabled={index === lessons.length - 1}
                                        className="btn-icon"
                                        title="Move down"
                                    >
                                        ↓
                                    </button>
                                </div>
                                <button onClick={() => openEditModal(lesson)} className="btn-link">✏️ Edit</button>
                                <button onClick={() => handleDelete(lesson.id)} className="btn-link danger">🗑️ Delete</button>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <h2>{editingLesson ? 'Edit Lesson' : 'Create New Lesson'}</h2>
                        <form onSubmit={editingLesson ? handleUpdate : handleCreate}>
                            <div className="form-group">
                                <label htmlFor="title">Lesson Title</label>
                                <input
                                    type="text"
                                    id="title"
                                    value={lessonTitle}
                                    onChange={(e) => setLessonTitle(e.target.value)}
                                    required
                                    placeholder="Enter lesson title"
                                    autoFocus
                                />
                            </div>
                            <div className="form-group">
                                <label htmlFor="order">Order</label>
                                <input
                                    type="number"
                                    id="order"
                                    value={lessonOrder}
                                    onChange={(e) => setLessonOrder(e.target.value)}
                                    required
                                    min="1"
                                    placeholder="Enter order number"
                                />
                            </div>
                            <div className="modal-actions">
                                <button type="button" onClick={() => setShowModal(false)} className="btn-secondary">
                                    Cancel
                                </button>
                                <button type="submit" className="btn-primary">
                                    {editingLesson ? 'Update' : 'Create'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Lessons;
