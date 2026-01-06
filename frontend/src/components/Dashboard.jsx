import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { courseService, authService } from '../services/api';
import './Dashboard.css';

function Dashboard() {
    const [stats, setStats] = useState({
        totalCourses: 0,
        publishedCourses: 0,
        draftCourses: 0,
        totalLessons: 0,
    });
    const [recentCourses, setRecentCourses] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        loadDashboardData();
    }, []);

    const loadDashboardData = async () => {
        try {
            setLoading(true);

            // Get all courses
            const allCourses = await courseService.searchCourses('', null, 1, 100);

            // Calculate stats
            const published = allCourses.items.filter(c => c.status === 1).length;
            const draft = allCourses.items.filter(c => c.status === 0).length;

            // Get total lessons count from recent courses
            let totalLessons = 0;
            const coursesWithDetails = await Promise.all(
                allCourses.items.slice(0, 5).map(async (course) => {
                    const summary = await courseService.getCourseSummary(course.id);
                    totalLessons += summary.totalLessons;
                    return summary;
                })
            );

            setStats({
                totalCourses: allCourses.items.length,
                publishedCourses: published,
                draftCourses: draft,
                totalLessons: totalLessons,
            });

            setRecentCourses(coursesWithDetails);
        } catch (err) {
            console.error('Error loading dashboard:', err);
        } finally {
            setLoading(false);
        }
    };

    const handleLogout = () => {
        authService.logout();
        navigate('/');
    };

    if (loading) {
        return (
            <div className="dashboard-container">
                <div className="loading">Loading dashboard...</div>
            </div>
        );
    }

    return (
        <div className="dashboard-container">
            <header className="dashboard-header">
                <h1>📊 Dashboard</h1>
                <div className="header-actions">
                    <span className="user-email">{authService.getEmail()}</span>
                    <button onClick={() => navigate('/courses')} className="btn-secondary">
                        📚 Courses
                    </button>
                    <button onClick={handleLogout} className="btn-secondary">Logout</button>
                </div>
            </header>

            <div className="stats-grid">
                <div className="stat-card total">
                    <div className="stat-icon">📚</div>
                    <div className="stat-content">
                        <h3>Total Courses</h3>
                        <p className="stat-number">{stats.totalCourses}</p>
                    </div>
                </div>

                <div className="stat-card published">
                    <div className="stat-icon">✅</div>
                    <div className="stat-content">
                        <h3>Published</h3>
                        <p className="stat-number">{stats.publishedCourses}</p>
                    </div>
                </div>

                <div className="stat-card draft">
                    <div className="stat-icon">📝</div>
                    <div className="stat-content">
                        <h3>Drafts</h3>
                        <p className="stat-number">{stats.draftCourses}</p>
                    </div>
                </div>

                <div className="stat-card lessons">
                    <div className="stat-icon">📖</div>
                    <div className="stat-content">
                        <h3>Total Lessons</h3>
                        <p className="stat-number">{stats.totalLessons}</p>
                    </div>
                </div>
            </div>

            <div className="recent-courses-section">
                <h2>Recent Courses</h2>
                <div className="recent-courses-list">
                    {recentCourses.map((course) => (
                        <div key={course.id} className="recent-course-card">
                            <div className="course-info">
                                <h3>{course.title}</h3>
                                <span className={`status-badge ${course.status === 1 ? 'published' : 'draft'}`}>
                                    {course.status === 1 ? 'Published' : 'Draft'}
                                </span>
                            </div>
                            <div className="course-stats">
                                <span className="lesson-count">📖 {course.totalLessons} lessons</span>
                                <span className="last-updated">
                                    Updated: {new Date(course.lastUpdated).toLocaleDateString()}
                                </span>
                            </div>
                            <button
                                onClick={() => navigate(`/courses/${course.id}/lessons`)}
                                className="btn-link"
                            >
                                View Lessons →
                            </button>
                        </div>
                    ))}
                </div>
            </div>

            <div className="quick-actions">
                <h2>Quick Actions</h2>
                <div className="actions-grid">
                    <button onClick={() => navigate('/courses')} className="action-card">
                        <span className="action-icon">➕</span>
                        <span className="action-text">Create New Course</span>
                    </button>
                    <button onClick={() => navigate('/courses')} className="action-card">
                        <span className="action-icon">📋</span>
                        <span className="action-text">View All Courses</span>
                    </button>
                </div>
            </div>
        </div>
    );
}

export default Dashboard;
