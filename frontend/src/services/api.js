const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5025/api';

const getAuthHeader = () => {
    const token = localStorage.getItem('token');
    return token ? { Authorization: `Bearer ${token}` } : {};
};

export const authService = {
    async register(email, password, confirmPassword) {
        const response = await fetch(`${API_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password, confirmPassword }),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Registration failed');
        }
        const data = await response.json();
        localStorage.setItem('token', data.token);
        localStorage.setItem('email', data.email);
        return data;
    },

    async login(email, password) {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password }),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Login failed');
        }
        const data = await response.json();
        localStorage.setItem('token', data.token);
        localStorage.setItem('email', data.email);
        return data;
    },

    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('email');
    },

    isAuthenticated() {
        return !!localStorage.getItem('token');
    },

    getEmail() {
        return localStorage.getItem('email');
    },
};

export const courseService = {
    async searchCourses(query = '', status = null, page = 1, pageSize = 10) {
        const params = new URLSearchParams({
            page: page.toString(),
            pageSize: pageSize.toString(),
        });
        if (query) params.append('q', query);
        if (status !== null) params.append('status', status.toString());

        const response = await fetch(`${API_URL}/courses/search?${params}`, {
            headers: getAuthHeader(),
        });
        if (!response.ok) throw new Error('Failed to fetch courses');
        return response.json();
    },

    async getCourseSummary(id) {
        const response = await fetch(`${API_URL}/courses/${id}/summary`, {
            headers: getAuthHeader(),
        });
        if (!response.ok) throw new Error('Failed to fetch course summary');
        return response.json();
    },

    async createCourse(title) {
        const response = await fetch(`${API_URL}/courses`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                ...getAuthHeader(),
            },
            body: JSON.stringify({ title }),
        });
        if (!response.ok) throw new Error('Failed to create course');
        return response.json();
    },

    async updateCourse(id, title) {
        const response = await fetch(`${API_URL}/courses/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                ...getAuthHeader(),
            },
            body: JSON.stringify({ title }),
        });
        if (!response.ok) throw new Error('Failed to update course');
        return response.json();
    },

    async deleteCourse(id) {
        const response = await fetch(`${API_URL}/courses/${id}`, {
            method: 'DELETE',
            headers: getAuthHeader(),
        });
        if (!response.ok) throw new Error('Failed to delete course');
    },

    async publishCourse(id) {
        const response = await fetch(`${API_URL}/courses/${id}/publish`, {
            method: 'PATCH',
            headers: getAuthHeader(),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Failed to publish course');
        }
    },

    async unpublishCourse(id) {
        const response = await fetch(`${API_URL}/courses/${id}/unpublish`, {
            method: 'PATCH',
            headers: getAuthHeader(),
        });
        if (!response.ok) throw new Error('Failed to unpublish course');
    },
};

export const lessonService = {
    async getLessonsByCourse(courseId) {
        const response = await fetch(`${API_URL}/courses/${courseId}/lessons`, {
            headers: getAuthHeader(),
        });
        if (!response.ok) throw new Error('Failed to fetch lessons');
        return response.json();
    },

    async createLesson(courseId, title, order) {
        const response = await fetch(`${API_URL}/courses/${courseId}/lessons`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                ...getAuthHeader(),
            },
            body: JSON.stringify({ title, order }),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Failed to create lesson');
        }
        return response.json();
    },

    async updateLesson(id, title, order) {
        const response = await fetch(`${API_URL}/lessons/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                ...getAuthHeader(),
            },
            body: JSON.stringify({ title, order }),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Failed to update lesson');
        }
        return response.json();
    },

    async deleteLesson(id) {
        const response = await fetch(`${API_URL}/lessons/${id}`, {
            method: 'DELETE',
            headers: getAuthHeader(),
        });
        if (!response.ok) throw new Error('Failed to delete lesson');
    },

    async reorderLesson(id, direction) {
        const response = await fetch(`${API_URL}/lessons/${id}/reorder`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                ...getAuthHeader(),
            },
            body: JSON.stringify({ direction }),
        });
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Failed to reorder lesson');
        }
    },
};
