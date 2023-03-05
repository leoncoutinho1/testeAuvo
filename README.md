# Teste Dev Pleno na Auvo

Neste teste criei uma aplicação MVC para cadastro de departamentos, empregados e horários de marcação de ponto atendendo a necessidade apresentada no requisito proposto onde uma funcionária do RH precisava calcular o pagamento devido por cada departamento com base na folha de ponto dos funcionários. As folhas de ponto de cada departamento são carregadas em uma página específica e posteriormente o relatório pode ser extraído.

## Construção

Utilizei .Net Core 6 e iniciei o projeto por meio do comando 

```bash
dotnet new mvc --auth Individual -o testeAuvo
```

Este comando cria o projeto e já instala a biblioteca do Identity. Essa lib possibilita protegermos as rotas por meio de permissões que podem ser atribuídas a usuários ou papéis.

Como o Identity tem dependência de uma persistência de dados ele já instala também o Entity Framework Core que usei para criar mais três classes: Employee, Departmente e ClockIn.

Criei as models normalmente utilizando as DataAnnotations para gerar a relação entre elas, inclui os DbSets no DbContext e rodei os comandos para gerar as migrations e atualizar o banco.

O banco Sqlite também já foi gerado durante a criação do projeto bastante apenas rodar os comandos:

```bash
dotnet ef migrations add "<descrição das classes criadas>"
dotnet ef database update
```

## Criando os Controllers automaticamente

Buscando a produtividade instalei o pacote Microsoft.VisualStudio.Web.CodeGenerator.Design por meio do comando:

```bash
dotnet tool install --global dotnet-aspnet-codegenerator
```

Com esse pacote instalado basta rodar o comando abaixo substittuindo o NameModel pelo nome de cada Model:

```bash
dotnet-aspnet-codegenerator controller -name <NameModel>Controller -dc ApplicationDbContext -m <NameModel> --useDefaultLayout --useSqlite --referenceScriptLibraries
```
O código autogerado criou o controllers e as views de CRUD necessárias para cada Model. Fiz o ajuste nas Views Create de Employee e ClockIn porque elas pediam o id da Departamento ou do Funcionário para realizar o cadastro. Troquei o campo Id pelo Name e realizei o ajuste no controller para buscar o Id a partir do Name e associar ao objeto que está sendo gravado.
Nas demais Views também ajustei para exibir o nome no lugar do Id.

Adicionei uma View no Load para o objeto ClockIn. Nela é feita a carga a partir dos arquivos .csv para popular o banco. A extração do relatório pode ser feita na tela ClockIn/Index.

## Contributing

Pull requests are welcome. For major changes, please open an issue first
to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)