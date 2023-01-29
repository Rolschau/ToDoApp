Vue.component('todo-component', {
    template: '#todo-component',
    props: {
        api: {
            type: String,
            default: "https://localhost:8000/todo"
        },
    },
    data() {
        return {
            todos: [],
            doneStatus: -1,
            isUpdatingData: false,
            isReordering: false,
            errorMessages: "",
            labels: {
                da: {
                    title: "Huskeliste",
                    status: "Status",
                    statusAll: "Alle",
                    statusDone: "Lukkede",
                    statusNotDone: "Åbne",
                    writeNewTodo: "Skriv en ny til listen...",
                    add: "Tilføj",
                    remove: "Fjern",
                },
                en: {
                    title: "ToDo list",
                    status: "Status",
                    statusAll: "All",
                    statusDone: "Closed",
                    statusNotDone: "Open",
                    writeNewTodo: "Type a new item...",
                    add: "Add",
                    remove: "Remove",
                },
            },
        }
    },
    computed: {
        alltodos() {
            return this.todos;
        },
        todoHide() {
            //'todoHide': (doneStatus > 0 && (todo.done && 1 == doneStatus || todo.done == false && todo.done == doneStatus))
            if (doneStatus == undefined || doneStatus == 0)
                return;
            return (doneStatus > 0 && (todo.done && 1 == doneStatus || todo.done == false && todo.done == doneStatus));
        },
    },
    watch: {
        hideDone() {
            console.log(`hideDone: ${hideDone.value ?? "undefined"}`);
        }
    },
    methods: {
        getAllToDo() {
            // Get all ToDo items
            const requestOptions = {
                method: "GET",
                headers: { "Accept": "application/json", "Content-Type": "application/json" }
            };
            fetch(`${this.api}/`, requestOptions)
                .then(async response => {
                    const data = await response.json();
                    if (!response.ok) {
                        // get error message from body or default to response status
                        const error = (data && data.message) || response.status;
                        return Promise.reject(error);
                    }
                    this.todos = data;
                })
                .catch(error => {
                    this.errorMessage = error;
                    console.error('There was an error in getToDos.', error);
                });
        },
        createToDo() {
            // Create a ToDo item
            const nextIndex = this.todos.length;
            const requestOptions = {
                method: "POST",
                headers: { "Accept": "application/json", "Content-Type": "application/json" },
                body: JSON.stringify({ value: this.$refs.newTodo.value, order: nextIndex })
            };
            fetch(`${this.api}/`, requestOptions)
                .then(async response => {
                    const data = await response.json();
                    if (!response.ok) {
                        // get error message from body or default to response status
                        const error = (data && data.message) || response.status;
                        return Promise.reject(error);
                    }
                    this.todos = [...this.todos, data];
                    this.$refs.newTodo.value = "";
                })
                .catch(error => {
                    this.errorMessage = error;
                    console.error('There was an error in createToDo.', error);
                });
        },
        doneToDo(index) {
            let todo = this.todos[index];
            let isDone = todo.done;
            this.todos[index].done = !isDone;
            this.updateToDo(todo);
        },
        updateToDo(todo) {
            this.isUpdatingData = true;
            // Update a ToDo whether it is done
            //todo.done = !todo.done; // Fix for v-model not affecting the todo item before it is passed to the method.
            const requestOptions = {
                method: "PUT",
                headers: { "Accept": "application/json", "Content-Type": "application/json" },
                body: JSON.stringify(todo)
            };
            fetch(`${this.api}/`, requestOptions)
                .then(async response => {
                    if (!response.ok) {
                        const error = response.status;
                        return Promise.reject(error);
                    }
                })
                .catch(error => {
                    this.errorMessage = error;
                    console.error('There was an error in updateToDo.', error);
                    todo.error = true;
                });
            this.isUpdatingData = false;
        },
        removeTodo(id) {
            //const requestOptions = {
            //    method: "DELETE",
            //    headers: { "Origin": location.href }
            //};
            fetch(`${this.api}/${id}`, { method: 'DELETE' })
                .then(() => {
                    const index = this.todos.map(todo => todo.id).indexOf(id);
                    (index >= 0) && this.todos.splice(index, 1);
                })
                .catch(error => {
                    this.errorMessage = error;
                    console.error('There was an error in updateToDo.', error);
                    todo.error = true;
                });
        },
        reorderTodo(todo, orderDirection) {
            this.isReordering = true;
            // Change the order of ToDo items.
            const index = this.todos.map(todo => todo.id).indexOf(todo.id);
            if (index < 0) return;
            const otherToDo = this.todos[index + orderDirection];
            const newCurrentToDo = { ...this.todos[index], order: otherToDo.order };
            const newOtherToDo = { ...otherToDo, order: todo.order };
            this.updateToDo(newCurrentToDo);
            this.updateToDo(newOtherToDo);

            let newTodosList = this.todos;
            newTodosList.splice(index, 1, newCurrentToDo);
            newTodosList.splice(index + orderDirection, 1, newOtherToDo);
            this.todos = this.getSortedTodos(newTodosList);
            this.isReordering = false;
        },
        getSortedTodos(todos) {
            return todos.sort((current, other) => {
                if (current.order < other.order) return -1;
                if (current.order > other.order) return 1;
                return 0;
            })
        },
        getLabel(key) {
            return this.labels[this.language][key];
        },
        setLanguage(cc) {
            this.language = cc;
        },
    },
    mounted() {
        this.getAllToDo();
    },
});