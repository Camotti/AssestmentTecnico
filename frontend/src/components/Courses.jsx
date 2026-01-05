import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { courseService, authService } from '../services/api';
import './Courses.css';

function Courses() {
    const [courses, setCourses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [searchQuery, setSearchQuery] = useState('');
    const [statusFilter, setStatusFilter] = useState('');
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [showModal, setShowModal] = useState(false);
    const [editingCourse, setEditingCourse] = useState(null);
    const [courseTitle, setCourseTitle] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        loadCourses();
    }, [page, statusFilter]);

    const loadCourses = async () => {
        try {
            setLoading(true);
            const status = statusFilter === '' ? null : parseInt(statusFilter);
            const data = await courseService.searchCourses(searchQuery, status, page, 10);
            setCourses(data.items);
            setTotalPages(data.totalPages);
            setError('');
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        e.preventDefault();
        setPage(1);
        loadCourses();
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            await courseService.createCourse(courseTitle);
            setShowModal(false);
            setCourseTitle('');
            loadCourses();
        } catch (err) {
            setError(err.message);
        }
    };

    const handleUpdate = async (e) => {
        e.preventDefault();
        try {
            await courseService.updateCourse(editingCourse.id, courseTitle);
            setShowModal(false);
            setEditingCourse(null);
            setCourseTitle('');
            loadCourses();
        } catch (err) {
            setError(err.message);
        }
    };

    const handleDelete = async (id) => {
        if (!confirm('Are you sure you want to delete this course?')) return;
        try {
            await courseService.deleteCourse(id);
            loadCourses();
        } catch (err) {
            setError(err.message);
        }
    };

    const handlePublish = async (id) => {
        try {
            await courseService.publishCourse(id);
            loadCourses();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleUnpublish = async (id) => {
        try {
            await courseService.unpublishCourse(id);
            loadCourses();
        } catch (err) {
            setError(err.message);
        }
    };

    const openCreateModal = () => {
        setEditingCourse(null);
        setCourseTitle('');
        setShowModal(true);
    };

    const openEditModal = (course) => {
        setEditingCourse(course);
        setCourseTitle(course.title);
        setShowModal(true);
    };

    const handleLogout = () => {
        authService.logout();
        navigate('/');
    };

    return (
        <div className="courses-container">
            <header className="courses-header">
                <h1>📚 My Courses</h1>
                <div className="header-actions">
                    <span className="user-email">{authService.getEmail()}</span>
                    <button onClick={handleLogout} className="btn-secondary">Logout</button>
                </div>
            </header>

            {error && <div className="error-message">{error}</div>}

            <div className="courses-controls">
                <form onSubmit={handleSearch} className="search-form">
                    <input
                        type="text"
                        placeholder="Search courses..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="search-input"
                    />
                    <button type="submit" className="btn-search">Search</button>
                </form>

                <select
                    value={statusFilter}
                    onChange={(e) => {
                        setStatusFilter(e.target.value);
                        setPage(1);
                    }}
                    className="status-filter"
                >
                    <option value="">All Status</option>
                    <option value="0">Draft</option>
                    <option value="1">Published</option>
                </select>

                <button onClick={openCreateModal} className="btn-primary">+ New Course</button>
            </div>

            {loading ? (
                <div className="loading">Loading...</div>
            ) : (
                <>
                    <div className="courses-grid">
                        {courses.map((course) => (
                            <div key={course.id} className="course-card">
                                <div className="course-header">
                                    <h3>{course.title}</h3>
                                    <span className={`status-badge ${course.status === 1 ? 'published' : 'draft'}`}>
                                        {course.status === 1 ? 'Published' : 'Draft'}
                                    </span>
                                </div>
                                <div className="course-actions">
                                    <button onClick={() => navigate(`/courses/${course.id}/lessons`)} className="btn-link">
                                        📖 Lessons
                                    </button>
                                    <button onClick={() => openEditModal(course)} className="btn-link">✏️ Edit</button>
                                    <button onClick={() => handleDelete(course.id)} className="btn-link danger">🗑️ Delete</button>
                                    {course.status === 0 ? (
                                        <button onClick={() => handlePublish(course.id)} className="btn-link success">
                                            ✅ Publish
                                        </button>
                                    ) : (
                                        <button onClick={() => handleUnpublish(course.id)} className="btn-link warning">
                                            ⏸️ Unpublish
                                        </button>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>

                    {totalPages > 1 && (
                        <div className="pagination">
                            <button
                                onClick={() => setPage(p => Math.max(1, p - 1))}
                                disabled={page === 1}
                                className="btn-secondary"
                            >
                                Previous
                            </button>
                            <span className="page-info">Page {page} of {totalPages}</span>
                            <button
                                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                                disabled={page === totalPages}
                                className="btn-secondary"
                            >
                                Next
                            </button>
                        </div>
                    )}
                </>
            )}

            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <h2>{editingCourse ? 'Edit Course' : 'Create New Course'}</h2>
                        <form onSubmit={editingCourse ? handleUpdate : handleCreate}>
                            <div className="form-group">
                                <label htmlFor="title">Course Title</label>
                                <input
                                    type="text"
                                    id="title"
                                    value={courseTitle}
                                    onChange={(e) => setCourseTitle(e.target.value)}
                                    required
                                    placeholder="Enter course title"
                                    autoFocus
                                />
                            </div>
                            <div className="modal-actions">
                                <button type="button" onClick={() => setShowModal(false)} className="btn-secondary">
                                    Cancel
                                </button>
                                <button type="submit" className="btn-primary">
                                    {editingCourse ? 'Update' : 'Create'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Courses;
