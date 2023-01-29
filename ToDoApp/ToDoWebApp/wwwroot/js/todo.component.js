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
            todoDoneStatus: undefined,
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
    },
    watch: {
        todoDoneStatus(newValue) {
            this.todos.forEach(
                (todo) => {
                    todo.hide = (newValue != undefined && todo.done == newValue)
                }
            );
        }
    },
    /*
    // Simple PUT request with a JSON body using fetch
    const requestOptions = {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: "Vue PUT Request Example" })
    };
    fetch(`${this.api}/`, requestOptions)
        .then(async response => {
        const isJson = response.headers.get('content-type').includes('application/json');
        const data = isJson && await response.json();
        
        // check for error response
        if (!response.ok) {
            // get error message from body or default to response status
            const error = (data && data.message) || response.status;
            return Promise.reject(error);
        }
        
        this.todos = data;
    })
    .catch(error => {
        this.errorMessage = error;
        console.error('There was an error!', error);
    });
    */
    methods: {
        loadTodos() {
            // loadTodos calls the api with GET to retrieve all todos.
            console.log(`${this.api}/`);
            const requestOptions = {
                method: "GET",
                headers: { "Content-Type": "application/json", "Origin": location.href },
            };
            fetch(`${this.api}/`, requestOptions)
                .then(async response => {
                    const isJson = response.headers.get('content-type').includes('application/json');
                    const data = isJson && await response.json();

                    // check for error response
                    if (!response.ok) {
                        debugger;
                        // get error message from body or default to response status
                        const error = (data && data.message) || response.status;
                        return Promise.reject(error);
                    }

                    this.todos = data;
                })
                .catch(error => {
                    debugger;
                    this.errorMessage = error;
                    console.error('There was an error!', error);
                });
        },
        addToDo() {
            // Oprette en opgave
            // POST https://jasonwatmore.com/post/2020/04/30/vue-fetch-http-post-request-examples
            console.log("ToDo: addToDo must call api POST to create");

            const todo = this.$refs.newTodo;
            const dirty_id = (new Date).getTime();
            this.todos = [...this.todos, { id: dirty_id, value: todo.value }];
            todo.value = "";
        },
        toggleToDo(todo) {
            console.log(todo);
            // Markere om en opgave som værende færdig
            // PUT https://jasonwatmore.com/post/2022/06/09/vue-fetch-http-put-request-examples
            console.log("ToDo: toggleToDo must call api PUT to update done status");
        },
        filterTodoStatus() {
            // Filtrer
            //this.todos = [...this.todos.filter(todo => todo.done)];
        },
        removeTodo(id) {
            // out of scope, but is needed...
            console.log("ToDo: addToDo must call api DELETE to delete");
            // DELETE https://jasonwatmore.com/post/2022/06/10/vue-fetch-http-delete-request-examples
            const index = this.todos.map(todo => todo.id).indexOf(id);
            (index >= 0) && this.todos.splice(index, 1);

        },
        getLabel(key) {
            return this.labels[this.language][key];
        },
        setLanguage(cc) {
            this.language = cc;
        },
    },
    mounted() {
        this.loadTodos();
        if (this.todos.length == 0)
            this.todos = [
                {
                    id: 1,
                    value: "Fake",
                    done: true
                }];
    },
});