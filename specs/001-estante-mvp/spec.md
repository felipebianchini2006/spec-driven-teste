# Feature Specification: Estante Virtual MVP

**Feature Branch**: `001-estante-mvp`  
**Created**: 2025-10-23  
**Status**: Draft  
**Input**: User description: "MVP da Estante Virtual - catalogar e avaliar livros lidos. Sistema anônimo (sem login) para adicionar livros com título, autor e capa, visualizar galeria de livros e avaliar com nota e resenha."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Adicionar Livro à Estante (Priority: P1)

Como usuário, quero adicionar um novo livro à minha estante informando título, autor e opcionalmente uma imagem da capa, para que eu possa começar a catalogar minha coleção de leituras.

**Why this priority**: Esta é a funcionalidade fundamental do sistema. Sem a capacidade de adicionar livros, não há estante virtual. É o primeiro passo necessário para qualquer outra funcionalidade e entrega valor imediato ao permitir que o usuário comece a organizar suas leituras.

**Independent Test**: Pode ser completamente testada acessando a aplicação, preenchendo o formulário de adição de livro com um título e autor, submetendo o formulário, e verificando que o livro foi persistido (permanece após recarregar a página).

**Acceptance Scenarios**:

1. **Given** a página inicial da estante virtual está aberta, **When** o usuário preenche o formulário com "1984" como título e "George Orwell" como autor e clica em adicionar, **Then** o livro "1984" de George Orwell é salvo e aparece na estante
2. **Given** o formulário de adicionar livro está visível, **When** o usuário preenche título "O Senhor dos Anéis", autor "J.R.R. Tolkien" e URL da capa "https://exemplo.com/capa.jpg", **Then** o livro é salvo com a imagem da capa associada
3. **Given** o formulário de adicionar livro está visível, **When** o usuário tenta submeter sem preencher o título, **Then** uma mensagem de validação indica que o título é obrigatório
4. **Given** o formulário de adicionar livro está visível, **When** o usuário tenta submeter sem preencher o autor, **Then** uma mensagem de validação indica que o autor é obrigatório
5. **Given** um livro foi adicionado com sucesso, **When** o usuário recarrega a página, **Then** o livro adicionado ainda aparece na estante (dados persistidos)

---

### User Story 2 - Visualizar Galeria de Livros (Priority: P2)

Como usuário, quero ver todos os livros que adicionei exibidos em uma galeria na página inicial, para que eu possa visualizar minha coleção completa de forma organizada e atrativa.

**Why this priority**: Depois de adicionar livros, o usuário precisa vê-los. Esta é a segunda funcionalidade mais importante pois transforma dados isolados em uma experiência visual significativa. Sem ela, o usuário não consegue realmente "usar" sua estante.

**Independent Test**: Pode ser testada adicionando múltiplos livros (utilizando a funcionalidade da User Story 1) e verificando que todos aparecem na página inicial em formato de galeria/grid, cada um mostrando capa (ou placeholder) e título.

**Acceptance Scenarios**:

1. **Given** três livros foram adicionados à estante, **When** o usuário acessa a página inicial, **Then** todos os três livros são exibidos em formato de galeria (grid)
2. **Given** um livro possui URL de imagem da capa válida, **When** o livro é exibido na galeria, **Then** a imagem da capa é mostrada
3. **Given** um livro NÃO possui URL de imagem da capa, **When** o livro é exibido na galeria, **Then** um placeholder de imagem padrão é mostrado
4. **Given** a estante está vazia (nenhum livro adicionado), **When** o usuário acessa a página inicial, **Then** uma mensagem indica que a estante está vazia e convida a adicionar o primeiro livro
5. **Given** múltiplos livros na galeria, **When** o usuário visualiza a página inicial, **Then** cada cartão de livro mostra claramente o título abaixo ou sobre a imagem

---

### User Story 3 - Avaliar Livro com Nota e Resenha (Priority: P3)

Como usuário, quero poder clicar em um livro da minha estante para ver seus detalhes e adicionar/editar uma avaliação com nota de 1 a 5 estrelas e um texto de resenha, para que eu possa registrar minha opinião sobre os livros que li.

**Why this priority**: Esta funcionalidade adiciona profundidade à experiência, transformando a estante de um simples catálogo em um diário de leitura pessoal. É uma funcionalidade de "polimento" que aumenta o valor mas não é essencial para o MVP básico funcionar.

**Independent Test**: Pode ser testada adicionando pelo menos um livro (User Story 1), clicando nele na galeria (User Story 2), acessando a página de detalhes, adicionando uma nota e resenha, salvando, e verificando que os dados são persistidos e exibidos corretamente.

**Acceptance Scenarios**:

1. **Given** um livro existe na estante, **When** o usuário clica nele na galeria, **Then** é navegado para uma página de detalhes mostrando título, autor e capa do livro
2. **Given** a página de detalhes de um livro está aberta, **When** o usuário seleciona uma nota de 4 estrelas e escreve "Excelente leitura!" na resenha, e salva, **Then** a nota e resenha são persistidas e exibidas na página de detalhes
3. **Given** um livro já possui uma avaliação (nota e resenha), **When** o usuário acessa a página de detalhes desse livro, **Then** a nota e resenha existentes são exibidas
4. **Given** a página de detalhes com avaliação existente está aberta, **When** o usuário edita a nota de 4 para 5 estrelas e atualiza a resenha, **Then** as mudanças são salvas e a avaliação atualizada é exibida
5. **Given** a página de detalhes está aberta, **When** o usuário tenta salvar uma avaliação sem selecionar uma nota, **Then** uma mensagem indica que a nota é obrigatória (a resenha pode ser opcional)

---

### Edge Cases

- **Título/Autor muito longos**: O que acontece quando um usuário insere um título ou nome de autor com 500 caracteres? Sistema deve limitar o tamanho (sugestão: 200 caracteres para título, 100 para autor) e exibir validação apropriada.
- **URL de imagem inválida**: Se o usuário fornece uma URL que não aponta para uma imagem válida, o sistema deve detectar e usar o placeholder padrão.
- **URL de imagem inacessível**: Se a URL era válida mas o servidor de imagens fica offline, deve-se usar placeholder como fallback.
- **Resenha muito longa**: Limitar tamanho da resenha (sugestão: 2000 caracteres) para evitar problemas de armazenamento e interface.
- **Caracteres especiais**: Títulos, autores e resenhas com caracteres especiais (emojis, caracteres Unicode) devem ser corretamente salvos e exibidos.
- **Livros duplicados**: Sistema deve permitir livros com mesmo título/autor (usuário pode ter lido o mesmo livro múltiplas vezes ou querer registrar edições diferentes).
- **Exclusão acidental**: Sem funcionalidade de deletar livros no MVP, este edge case não se aplica ainda.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Sistema DEVE permitir adicionar um novo livro informando título e autor (campos obrigatórios) e URL de imagem da capa (campo opcional)
- **FR-002**: Sistema DEVE validar que título e autor foram preenchidos antes de permitir salvar um livro
- **FR-003**: Sistema DEVE limitar o tamanho dos campos de texto (título: 200 caracteres, autor: 100 caracteres, URL: 500 caracteres)
- **FR-004**: Sistema DEVE persistir todos os livros adicionados de forma que permaneçam disponíveis após recarregar a aplicação
- **FR-005**: Sistema DEVE exibir todos os livros cadastrados em formato de galeria/grid na página inicial
- **FR-006**: Sistema DEVE mostrar a imagem da capa do livro quando uma URL válida foi fornecida
- **FR-007**: Sistema DEVE exibir um placeholder de imagem padrão quando nenhuma URL de capa foi fornecida ou quando a URL é inválida/inacessível
- **FR-008**: Sistema DEVE exibir o título de cada livro junto com sua capa na galeria
- **FR-009**: Sistema DEVE exibir uma mensagem apropriada quando a estante está vazia (nenhum livro cadastrado)
- **FR-010**: Sistema DEVE permitir clicar em um livro da galeria para navegar para sua página de detalhes
- **FR-011**: Página de detalhes DEVE exibir título, autor e imagem da capa do livro selecionado
- **FR-012**: Página de detalhes DEVE permitir adicionar uma avaliação composta por nota (1 a 5 estrelas, obrigatória) e resenha (texto livre, opcional)
- **FR-013**: Sistema DEVE limitar o tamanho da resenha a 2000 caracteres
- **FR-014**: Sistema DEVE permitir editar uma avaliação existente (nota e/ou resenha)
- **FR-015**: Sistema DEVE persistir avaliações de forma que permaneçam disponíveis após recarregar a aplicação
- **FR-016**: Sistema DEVE exibir a avaliação existente (nota e resenha) na página de detalhes do livro

### Assumptions

- **A-001**: Como não há sistema de login, todos os dados são armazenados localmente no navegador do usuário ou em um banco de dados compartilhado único (decisão de implementação). Para o MVP, assume-se que é aceitável que dados sejam perdidos ao limpar cache do navegador se usar armazenamento local.
- **A-002**: O sistema é single-user: não há necessidade de suportar múltiplos usuários simultâneos com estantes separadas neste MVP.
- **A-003**: URLs de imagens devem ser públicas e acessíveis via HTTP/HTTPS. Não há upload de arquivos no MVP.
- **A-004**: Não há funcionalidade de busca, filtro ou ordenação de livros no MVP - todos os livros são mostrados em ordem de adição.
- **A-005**: Não há funcionalidade de editar ou excluir livros no MVP - apenas adicionar e avaliar.
- **A-006**: A nota de avaliação é obrigatória mas a resenha é opcional ao salvar uma avaliação.
- **A-007**: Cada livro pode ter no máximo uma avaliação (não há histórico de múltiplas avaliações).
- **A-008**: O sistema suportará caracteres Unicode (incluindo caracteres especiais e emojis) em todos os campos de texto.

### Key Entities

- **Livro**: Representa um livro catalogado na estante. Atributos: identificador único, título (texto obrigatório, máx 200 chars), autor (texto obrigatório, máx 100 chars), URL da imagem da capa (texto opcional, máx 500 chars), data de adição. Relacionamento: possui zero ou uma Avaliação.

- **Avaliação**: Representa a avaliação de um livro pelo usuário. Atributos: identificador único, nota (número inteiro de 1 a 5, obrigatório), resenha (texto opcional, máx 2000 chars), data de criação, data de última edição. Relacionamento: pertence a exatamente um Livro.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Usuários podem adicionar um novo livro completo (título, autor e opcionalmente capa) em menos de 30 segundos
- **SC-002**: Galeria de livros exibe todos os livros cadastrados em menos de 2 segundos após carregar a página inicial
- **SC-003**: Usuários conseguem navegar para a página de detalhes de um livro com um único clique
- **SC-004**: Usuários podem adicionar uma avaliação completa (nota e resenha) em menos de 1 minuto
- **SC-005**: Sistema mantém 100% dos dados persistidos após recarregar a página (sem perda de informação)
- **SC-006**: Imagens de capa (quando URL válida fornecida) são exibidas em menos de 3 segundos
- **SC-007**: 95% dos usuários conseguem adicionar seu primeiro livro na primeira tentativa sem erros de validação (assumindo que preenchem campos obrigatórios)
- **SC-008**: Sistema suporta catálogo de até 100 livros sem degradação visível de performance na galeria
- **SC-009**: Taxa de erros de validação é inferior a 10% das tentativas de submissão de formulários
- **SC-010**: Usuários encontram a funcionalidade de avaliação em menos de 10 segundos após visualizar a galeria (boa descoberta da UI)
